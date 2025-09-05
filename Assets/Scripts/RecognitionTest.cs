
using PDollarGestureRecognizer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


public class RecognitionTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform gestureOnScreenPrefab;

    [SerializeField] private Transform[] objToInstantiate;

    private List<Gesture> trainingSet = new List<Gesture>();

    private List<Point> points = new List<Point>();
    private int strokeId = -1;

    private Vector2 virtualKeyPosition = Vector2.zero;

    private Rect drawArea;

    private RuntimePlatform platform;
    private int vertexCount = 0;

    private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
    private LineRenderer currentGestureLineRenderer;

    private bool recognized;

    [SerializeField] private InputActionReference drawAction;
    [SerializeField] private InputActionReference toggleDrawAction;
    [SerializeField] private InputActionReference mousePos;

    private void OnEnable()
    {
        drawAction.action.Enable();
        toggleDrawAction.action.Enable();
        mousePos.action.Enable();
        toggleDrawAction.action.performed += OnToggleDraw;
    }

    private void OnDisable()
    {
        drawAction.action.Disable();
        toggleDrawAction.action.Disable();
        mousePos.action.Disable();
        toggleDrawAction.action.performed -= OnToggleDraw;
    }



    void Start()
    {

        platform = Application.platform;
        drawArea = new Rect(0, 0, Screen.width, Screen.height);

        //Load pre-made gestures
        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

        //Load user custom gestures
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (string filePath in filePaths)
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
    }
    

    private void Update()
    {
        if (drawAction.action.IsPressed())
        {

            OnDraw();
        }
    }


    private void OnDraw()
    {
        print("In On Draw function");
        //virtualKeyPosition = new Vector2(mousePos.action.ReadValue<Vector2>().x, mousePos.action.ReadValue<Vector2>().y);
        virtualKeyPosition = new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y);
        if (drawArea.Contains(virtualKeyPosition)) // make it a return statement.
        {
            if (drawAction.action.WasPressedThisFrame())
            {
                if (recognized)
                {
                    recognized = false;
                    strokeId = -1;

                    points.Clear();

                    foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                    {
                        lineRenderer.positionCount = 0;
                        Destroy(lineRenderer.gameObject);
                    }

                    gestureLinesRenderer.Clear();
                }

                ++strokeId;

                Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
                currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                gestureLinesRenderer.Add(currentGestureLineRenderer);

                vertexCount = 0;
            }
            points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

            currentGestureLineRenderer.positionCount = ++vertexCount;
            //currentGestureLineRenderer.SetPosition(vertexCount - 1, new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)); Draws it on canvas ish, may be better.
            currentGestureLineRenderer.SetPosition(vertexCount - 1,  Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
        }
    }

    private void OnToggleDraw(InputAction.CallbackContext context) // rn just recognize but later will disable drawing on and off. 
    {
        Recognize();
    }

    void Recognize()
    {
        recognized = true;

        Gesture candidate = new Gesture(points.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

        if (gestureResult.Score < 0.9)
        {
            print("Not accurate enough");
            return;
        }

        print(gestureResult.GestureClass + " " + gestureResult.Score);

        foreach (Transform obj in objToInstantiate)
        {
            print(obj.name);
            print(gestureResult.GestureClass);
            print(gestureResult.GestureClass.Equals(obj.name));
            if (obj.name == gestureResult.GestureClass)
            {
                Instantiate(obj);
                print("Succesfull");
            }
        }
    }
}


