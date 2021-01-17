using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Visualizer : MonoBehaviour
{
    public LSystemGenerator lsystem;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    public Material lineMaterial;

    [SerializeField]
    private int length = 8;
    [SerializeField]
    private float angle = 90;

    public int Length
    {
        get
        {
            if (length > 0) { return length; }
            else
            {  return 1; //possibly random
                //Random r = new Random();
                //return r.Next(1, 3);
            }

            
        }
        set => length = value;
    }

    private void VisualizeSeq(string seq) {
        Stack<AgentParamaters> savePoints = new Stack<AgentParamaters>();
        var curPos = Vector3.zero;
        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;

        positions.Add(curPos);

        foreach (var letter in seq)
        {
            letters l = (letters)letter;
            switch (l)
            {
                 
                    case letters.save:
                    savePoints.Push(new AgentParamaters
                    {
                        position = curPos,
                        direction = direction,
                        length=length
                    });

                    break;

                case letters.load:
                    if(savePoints.Count != 0)
                    {
                        var age = savePoints.Pop();
                        curPos = age.position;
                        direction = age.direction;
                        length = age.length;
                    }
                    break;
                case letters.draw:
                    tempPosition = curPos;
                    curPos += direction * length;
                    DrawLine(tempPosition, curPos, Color.red);
                    //Random r = new Random();
                    //length -= r.Next(1,3); //2
                    length -= 2;
                    positions.Add(curPos);
                    break;
                case letters.right:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    break;

                case letters.left:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    break;
                default:
                    break;
            }
        }

        foreach (var pos in positions)
        {
            Instantiate(prefab, pos, Quaternion.identity);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color red)
    {
        GameObject line = new GameObject("line");
        line.transform.position = start;
        var lr = line.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startColor = lr.endColor = red;
        lr.startWidth = lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    public enum letters
    {
        unknown = '1',
        save= '[',
        load=']',
        draw = 'F',
        right = '+',
        left='-'
    }
    // Start is called before the first frame update
    void Start()
    {
        var sequence = lsystem.GenerateSentance();
        VisualizeSeq(sequence);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
