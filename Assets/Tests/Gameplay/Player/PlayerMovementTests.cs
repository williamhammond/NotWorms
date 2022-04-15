using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.Player
{
    public class PlayerMovementTests
    {
        [UnityTest]
        public IEnumerator PlayerMovesRight()
        {
            RenderSettings.skybox = null;

            var root = new GameObject();
            root.AddComponent(typeof(Camera));

            var camera = root.GetComponent<Camera>();
            camera.backgroundColor = Color.white;

            root = Object.Instantiate(root);

            var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
            player = Object.Instantiate(player, new Vector2(0, 0), new Quaternion(0, 0, 0, 0));
            player.SendMessage("MoveHorizontal");

            yield return new WaitForSeconds(3f);

            Assert.IsTrue(player.transform.position.x == 0);

            Object.Destroy(player);
            Object.Destroy(root);
        }
    }
}
