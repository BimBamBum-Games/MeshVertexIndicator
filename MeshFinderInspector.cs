using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MeshVertexFinder))]
public class MeshFinderInspector : Editor
{
    //Mesh Noktalari Analiz Eden Sinif
    MeshVertexFinder _mFinder;
    Vector3[] _vertices;
    int[] _triangles;
    public int fontSize = 10;
    private bool isPrint = false;

    [Serializable]
    public class VertexIndentifier
    {
        //Mesh Birlestirici Veri Yapisi
        public Vector3 pos;
        public int triangleIndex;
        public int tIndex;
        public bool IsChecked { get; set; }
        public int GrupNumber { get; set; }

        public VertexIndentifier(Vector3 pos, int triangleIndex, int tIndex)
        {
            this.pos = pos;
            this.triangleIndex = triangleIndex;
            this.tIndex = tIndex;
            GrupNumber = tIndex;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Recovery"))
        {
            _mFinder.Recovery();
        }
    }

    public void OnSceneGUI()
    {
        _mFinder = (MeshVertexFinder)target;
        _vertices = _mFinder.meshVertices;
        _triangles = _mFinder.meshTriangles;

        //Once Vertices ve Triangles lari VertexIdentifier Class Veri Yapisinda Birlestirilir. Hesaplamalarda Kolaylik Saglanir.
        List<VertexIndentifier> vertexTriangle = new List<VertexIndentifier>();
        for(int i = 0; i < _vertices.Length; i++)
        {
            VertexIndentifier vi = new VertexIndentifier(_vertices[i], i, i);
            vertexTriangle.Add(vi);
            if(isPrint == true)
            {
                PrintSpecs(vi);
            }        
        }

        //Ayni Pozisyona Sahip Vertexleri Bul ve Tek Bir Nokta Seklinde Degerlendir.
        //Kontrol Edildi mi Edilmedi mi Diye Isleme Alninan Vertexler Tekrar Isleme Alinmaz Performans Saglanir.
        int groupIndex = 0;
        for(int i = 0; i < vertexTriangle.Count; i++)
        {
            VertexIndentifier vi = vertexTriangle[i];
            if(vi.IsChecked == false)
            {               
                vi.GrupNumber = groupIndex;
                for (int j = 0; j < vertexTriangle.Count; j++)
                {
                    if (vertexTriangle[j].IsChecked == false && vi.pos == vertexTriangle[j].pos)
                    {
                        vertexTriangle[j].IsChecked = true;
                        vertexTriangle[j].GrupNumber = vi.GrupNumber;
                    }
                }
                vi.IsChecked = true;
                groupIndex++;           
            }
        }

        //Ayni Konumu Paylasan Vertexleri Grup Olarak O konumda Ekrana Yazdir.
        for(int g = 0; g < groupIndex; g++)
        {
            string str = "";
            List<VertexIndentifier> viList = new();
            for(int i = 0; i < vertexTriangle.Count; i++)
            {
                if (vertexTriangle[i].GrupNumber== g)
                {
                    str += i.ToString() + " ";
                    viList.Add(vertexTriangle[i]);
                }
            }

            float size = HandleUtility.GetHandleSize(_mFinder.transform.TransformPoint(viList[0].pos));
            Handles.DotHandleCap(0, _mFinder.transform.TransformPoint(viList[0].pos), Quaternion.identity, _mFinder.vertexScale * size, EventType.Repaint);
            GUIStyle gstyle = new();
            gstyle.fontSize = fontSize;
            gstyle.normal.textColor = Color.white;
            Handles.Label(_mFinder.transform.TransformPoint(viList[0].pos), str, gstyle);
        }
    }

    public void PrintSpecs(VertexIndentifier vertexTriangle)
    {
        //Mesh Hakkinda Ekrana Bilgi Yazdirma Metodudur.
        Debug.Log
                ($"Vector : {vertexTriangle.pos} " +
                $"TriangleIndex : {vertexTriangle.triangleIndex} " +
                $"Grup : {vertexTriangle.GrupNumber} " +
                $"Index : {vertexTriangle.tIndex} " +
                $"Status : {vertexTriangle.IsChecked}");
    }

    public List<Vector3> FindClosestVertices(int index, bool isInfo = false)
    {
        //Verilen vertex indexindeki ona en yakin olan vertexleri bulmasini hedefler. Ayni nokta vertex eger sekil FBS ise 3, eger Object ise 2 vertex icerir.
        Vector3 currentVertex = _vertices[index];
        List<Vector3> closestVertices = new();
        for(int i = 0; i < _vertices.Length; i++) 
        {
            float magnit = (_vertices[i] - currentVertex).magnitude;
            if(magnit < 0.0001f)
            {
                closestVertices.Add(_vertices[i]);
            }
        }

        if(isInfo)
        {
            string strInterpolation = $"Vertex[{index}] >>>>> Vertex Count : {closestVertices.Count}";
            Debug.Log(strInterpolation);
        }

        return closestVertices;
    }
}
