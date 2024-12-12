using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private CharacterController _cc;

    private float playerSpeed = 2f;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork() // FixedUpdate 네트워크 상에서 움직임은 다 여기다가 처리를 해줄거에요.
    {
        // base.FixedUpdateNetwork();

        if (HasStateAuthority == false) return; // 이 플레이어를 보면 우리가 생성한 오브젝트에 대한 권한이 저한테 있어요. 
                                                // 근데 다른 클라이언트가 생성한 플레이어는 우리가 권한이 없어요. 
                                                // 이 권한이 되게 중요한데, 일단 밑에 코드를 작성해 볼게요.
                                                // 우리가 입력을 했을때, 다른 클라이언트의 플레이어 가 움직이면 안되잖아요.
                                                // 그래서 네트워크 오브젝트들은 다 권한이 있어요. 

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * playerSpeed;

        _cc.Move(move);

        if(move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
}
