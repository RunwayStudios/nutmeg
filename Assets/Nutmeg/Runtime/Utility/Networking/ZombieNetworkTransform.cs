using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Networking
{
    [DefaultExecutionOrder(100000)] // this is needed to catch the update time after the transform was updated by user scripts
    public class ZombieNetworkTransform : NetworkBehaviour
    {
        [Tooltip("Speed in m/s when interpolating between the current position and the most recent server state")] [SerializeField]
        private float interpolationSpeed = 10f;

        [Tooltip("Maximum distance between the current position and a new server state above which interpolation is skipped")] [SerializeField]
        private float teleportationThreshold = 1f;


        private ServerState curServerState;
        private ServerState prevServerState;
        private bool stopped;
        private bool interpolating;
        private Vector3 interpolationStartPos;
        private float interpolationStartTime;
        private float interpolationDistance;
        private bool predicting;
        private Vector3 prevVelocity;


        public void UpdateServerState(bool hasStopped = false)
        {
            if (!IsServer && !IsHost)
                return;

            UpdateServerStateClientRpc(new ServerState(transform.position, transform.rotation.eulerAngles.y, Time.time), hasStopped);
        }


        [ClientRpc]
        private void UpdateServerStateClientRpc(ServerState newServerState, bool hasStopped)
        {
            if (IsServer || IsHost)
                return;

            prevServerState = curServerState;
            curServerState = newServerState;

            bool oldStopped = stopped;
            stopped = hasStopped;

            interpolationDistance = Vector3.Distance(transform.position, curServerState.pos);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, curServerState.rotY, transform.rotation.eulerAngles.z);
            
            if (oldStopped && !stopped)
            {
                interpolating = false;
                StartPredicting(true);
            }
            else if (interpolationDistance < 0.01f)
            {
                interpolating = false;
                StartPredicting();
            }
            else
            {
                StartInterpolating();
                predicting = false;
            }
        }

        private void StartInterpolating()
        {
            interpolating = true;
            interpolationStartPos = transform.position;
            interpolationStartTime = Time.time;
        }

        private void StartPredicting(bool skipVelocityCalculation = false)
        {
            predicting = true;
            if (!skipVelocityCalculation && curServerState.serverTime - prevServerState.serverTime > 0)
                prevVelocity = (curServerState.pos - prevServerState.pos) / (curServerState.serverTime - prevServerState.serverTime);
        }


        // Update is called once per frame
        void Update()
        {
            if (IsServer || IsHost)
            {
                // todo send state
            }
            else if (interpolating)
            {
                Vector3 serverPos = curServerState.pos;
                Vector3 curPos = transform.position;
                if (Mathf.Abs(serverPos.x - curPos.x) >= teleportationThreshold ||
                    Mathf.Abs(serverPos.y - curPos.y) >= teleportationThreshold ||
                    Mathf.Abs(serverPos.z - curPos.z) >= teleportationThreshold)
                {
                    transform.position = serverPos;
                    interpolating = false;
                    StartPredicting();
                }
                else
                {
                    float timePassed = Time.time - interpolationStartTime;
                    transform.position = Vector3.Lerp(interpolationStartPos, serverPos, timePassed / (interpolationDistance / interpolationSpeed));

                    if (transform.position == serverPos)
                    {
                        interpolating = false;
                        StartPredicting();
                    }
                }
            }
            else if (predicting)
            {
                if (stopped)
                {
                    predicting = false;
                    return;
                }

                transform.position += prevVelocity * Time.deltaTime;
            }
        }
    }


    readonly struct ServerState : INetworkSerializeByMemcpy
    {
        public readonly Vector3 pos;
        public readonly float rotY;
        public readonly float serverTime;

        public ServerState(Vector3 pos, float rotY, float serverTime)
        {
            this.pos = pos;
            this.rotY = rotY;
            this.serverTime = serverTime;
        }
    }
}