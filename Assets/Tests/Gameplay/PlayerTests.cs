﻿using System.Collections;
using Gameplay;
using NSubstitute;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.Gameplay
{
    public class PlayerTest
    {
        public class PlayerMovementTests
        {
            [UnityTest]
            public IEnumerator PlayerMovesRight()
            {
                RenderSettings.skybox = null;

                var root = new GameObject();
                root = Object.Instantiate(root);
                var player = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/Player.prefab"
                );
                player = Object.Instantiate(player, new Vector2(0, 0), new Quaternion(0, 0, 0, 0));

                Player comp = player.GetComponent<Player>();
                comp.PlayerInput = Substitute.For<IPlayerInput>();
                comp.PlayerInput.Horizontal.Returns(10f);

                yield return WaitForNFrames(60);

                Assert.IsTrue(player.transform.position.x > 0);

                Object.Destroy(player);
                Object.Destroy(root);
            }

            [UnityTest]
            public IEnumerator PlayerMovesLeft()
            {
                RenderSettings.skybox = null;

                var root = new GameObject();
                root = Object.Instantiate(root);
                var player = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/Player.prefab"
                );
                player = Object.Instantiate(player, new Vector2(0, 0), new Quaternion(0, 0, 0, 0));

                Player comp = player.GetComponent<Player>();
                comp.PlayerInput = Substitute.For<IPlayerInput>();
                comp.PlayerInput.Horizontal.Returns(-10f);

                yield return WaitForNFrames(60);

                Assert.IsTrue(player.transform.position.x < 0);

                Object.Destroy(player);
                Object.Destroy(root);
            }

            [UnityTest]
            public IEnumerator PlayerJumps()
            {
                RenderSettings.skybox = null;

                var root = new GameObject();
                root = Object.Instantiate(root);
                var player = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/Player.prefab"
                );
                player = Object.Instantiate(player, new Vector2(0, 0), new Quaternion(0, 0, 0, 0));

                Player comp = player.GetComponent<Player>();
                comp.PlayerInput = Substitute.For<IPlayerInput>();

                comp.PlayerInput.IsJumping.Returns(true);
                yield return WaitForNFrames(120);
                comp.PlayerInput.IsJumping.Returns(true);

                Assert.IsTrue(player.transform.position.y > 0);

                Object.Destroy(player);
                Object.Destroy(root);
            }

            private IEnumerator WaitForNFrames(int n)
            {
                for (int i = 0; i < n; i++)
                {
                    yield return null;
                }
            }
        }
    }
}
