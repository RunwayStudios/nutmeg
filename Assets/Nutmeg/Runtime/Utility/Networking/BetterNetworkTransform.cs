using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Networking
{
    [DefaultExecutionOrder(100000)] // this is needed to catch the update time after the transform was updated by user scripts
    public class BetterNetworkTransform : NetworkBehaviour
    {
        [SerializeField] private bool check;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (check)
            {
                Debug.Log("server: " + IsServer);
                Debug.Log("host: " + IsHost);
                Debug.Log("client: " + IsClient);
            }
        }
    }
}