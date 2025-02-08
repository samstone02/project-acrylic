using UnityEngine;

public abstract class BaseObjective : MonoBehaviour
{
    public float Timer {  get; set; }

    private float _timer { get; set; }

    private void Start()
    {
        _timer = Timer;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            StartObjective();
        }
    }

    protected abstract void StartObjective();

    protected abstract void OnCapture();

    public void OnTriggerEnter(Collider other)
    {
        var tank = other.GetComponent<Tank>();

        if (tank == null)
        {
            return;
        }

        //if ()
    }
}