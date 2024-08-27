using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;

        [Header("debug join")]
        [SerializeField] bool startGameAsClient;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;

                // Najpierw musimy zamknąć bieżącą sesję sieciową, ponieważ gra została uruchomiona jako host na ekranie tytułowym
                NetworkManager.Singleton.Shutdown();

                // Następnie ponownie uruchamiamy menedżera sieciowego, tym razem jako klient
                NetworkManager.Singleton.StartClient();
            }
        }

    
}