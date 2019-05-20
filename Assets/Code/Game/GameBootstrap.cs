//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.Assertions;

//// TODOING: make this work :)
//// based off of https://github.com/Unity-Technologies/EntityComponentSystemSamples/issues/127
//public sealed class GameBootstrap
//{
//    public static void NewGame()
//    {
//        //var player = Object.Instantiate(PlayerPrefab);
//        //var playerEntity = player.GetComponent<GameObjectEntity>().Entity;
//        EntityManager entityManager = World.Active.EntityManager;

//        Camera mainCamera = Camera.main;
//        if (mainCamera == null) {
//            return;
//        }

//        Entity cameraEntity = mainCamera.GetComponent<GameObjectEntity>().Entity;

//        entityManager.AddComponentData(cameraEntity, new CameraData());
//        entityManager.AddComponentData(cameraEntity, new Translation());
//        entityManager.AddComponentData(cameraEntity, new Rotation());
//    }
//}
