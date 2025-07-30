using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ClickMovementCC : MonoBehaviour
{
    [SerializeField] private GameObject _clickPositionVisualizer;
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        if(_navMeshAgent == null)
        {
            Debug.LogError("Nav Mesh Agent is NULL");
        }
    }

    void Update()
    {
        MoveToClick();
    }

    private void MoveToClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if(Physics.Raycast(ray, out raycastHit))
            {
                _navMeshAgent.destination = raycastHit.point;

                _clickPositionVisualizer.transform.position = raycastHit.point;
            }
        }
    }
}
