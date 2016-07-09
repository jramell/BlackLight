using UnityEngine;
using System;

/// <summary>
/// Contains the properties of the painted polygon
/// </summary>
public class PolygonPainterEventArgs : EventArgs {

	private Vector3[] points;

	public Vector3[] Points {
		get {
			return points;
		}
	}

	private int pointCount;

	public int PointCount {
		get {
			return pointCount;
		}
	}

	public PolygonPainterEventArgs(Vector3[] points, int pointCount)
	{
		this.points = points;
		this.pointCount = pointCount;
	}
}
