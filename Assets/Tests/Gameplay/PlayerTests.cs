using System.Collections;

namespace Tests.Gameplay
{
    public class PlayerTest
    {
        public class PlayerMovementTests
        {
            // [UnityTest]
            // [Timeout(1000 * 5)]
            // public IEnumerator PlayerMovesRight()
            // {
            //     RenderSettings.skybox = null;

            //     var root = new GameObject();
            //     root = Object.Instantiate(root);
            //     var player = AssetDatabase.LoadAssetAtPath<GameObject>(
            //         "Assets/Prefabs/Player.prefab"
            //     );
            //     player = Object.Instantiate(player, new Vector2(0, 0), new Quaternion(0, 0, 0, 0));

            //     Player comp = player.GetComponent<Player>();
            //     comp.PlayerInput = Substitute.For<IPlayerInput>();

            //     comp.PlayerInput.HorizontalMovement.Returns(10f);
            //     yield return new WaitUntil(() => player.transform.position.x > 0);

            //     Object.Destroy(player);
            //     Object.Destroy(root);
            // }

            // [UnityTest]
            // [Timeout(1000 * 5)]
            // public IEnumerator PlayerMovesLeft()
            // {
            //     RenderSettings.skybox = null;

            //     var root = new GameObject();
            //     root = Object.Instantiate(root);
            //     var player = AssetDatabase.LoadAssetAtPath<GameObject>(
            //         "Assets/Prefabs/Player.prefab"
            //     );
            //     player = Object.Instantiate(player, new Vector2(0, 0), new Quaternion(0, 0, 0, 0));

            //     Player comp = player.GetComponent<Player>();
            //     comp.PlayerInput = Substitute.For<IPlayerInput>();

            //     comp.PlayerInput.HorizontalMovement.Returns(-10f);
            //     yield return new WaitUntil(() => player.transform.position.x < 0);

            //     Object.Destroy(player);
            //     Object.Destroy(root);
            // }

            // [UnityTest]
            // [Timeout(1000 * 5)]
            // public IEnumerator PlayerJumps()
            // {
            //     RenderSettings.skybox = null;

            //     var root = new GameObject();
            //     root = Object.Instantiate(root);
            //     var player = AssetDatabase.LoadAssetAtPath<GameObject>(
            //         "Assets/Prefabs/Player.prefab"
            //     );
            //     player = Object.Instantiate(player, new Vector2(0, 0), new Quaternion(0, 0, 0, 0));

            //     Player comp = player.GetComponent<Player>();
            //     comp.PlayerInput = Substitute.For<IPlayerInput>();

            //     comp.PlayerInput.Jump.Returns(true);

            //     yield return new WaitUntil(() => player.transform.position.y > 0);

            //     Object.Destroy(player);
            //     Object.Destroy(root);
            // }

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
