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
        private const int MaxFramesPerTest = 60 * 5;

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

                bool hasMoved = false;
                int frames = 0;
                while (!hasMoved && frames < MaxFramesPerTest)
                {
                    yield return WaitForNFrames(1);
                    if (player.transform.position.x > 0)
                    {
                        hasMoved = true;
                    }
                    frames++;
                }
                Assert.IsTrue(hasMoved, "Player failed to move right");

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

                bool hasMoved = false;
                int frames = 0;
                while (!hasMoved && frames < MaxFramesPerTest)
                {
                    yield return WaitForNFrames(1);
                    if (player.transform.position.x < 0)
                    {
                        hasMoved = true;
                    }
                    frames++;
                }
                Assert.IsTrue(hasMoved, "Player failed to move left");

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
                yield return new WaitForSeconds(1f);
                bool hasJumped = false;
                int frames = 0;
                while (!hasJumped && frames < MaxFramesPerTest)
                {
                    yield return WaitForNFrames(1);
                    if (player.transform.position.y > 0)
                    {
                        hasJumped = true;
                    }
                    frames++;
                }
                comp.PlayerInput.IsJumping.Returns(false);

                Assert.IsTrue(hasJumped, "Player failed to jump");

                Object.Destroy(player);
                Object.Destroy(root);
            }

            private IEnumerator WaitForNFrames(int n)
            {
                // Do not use WaitForEndOfFrame() here until Unity fixes coroutine issues
                // with the method https://forum.unity.com/threads/do-not-use-waitforendofframe.883648/
                for (int i = 0; i < n; i++)
                {
                    yield return null;
                }
            }
        }
    }
}