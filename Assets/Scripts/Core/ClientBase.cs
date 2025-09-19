using UnityEngine;

public abstract class ClientBase : NPCBase
{
    [SerializeField] protected float patience = 60f;
    [SerializeField] protected bool served;

    public bool Served => served;

    public override void Tick(float dt)
    {
        if (!served)
        {
            patience -= dt;
            if (patience <= 0f)
            {
                OnLeaveUnserved();
            }
        }
    }

    protected virtual void OnLeaveUnserved()
    {
        // default behavior: destroy; production game may pool instead
        gameObject.SetActive(false);
    }
}
