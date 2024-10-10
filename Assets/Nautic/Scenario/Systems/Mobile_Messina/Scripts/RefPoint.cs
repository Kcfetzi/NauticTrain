
using Unity.Mathematics;
using UnityEngine;

// refactor
public class RefPoint : MonoBehaviour
{
    public double Lat;
    public double Lon;
    public Position Position;

    private void Awake()
    {
        Position = new Position(Lat, Lon, transform.position);
    }
}
