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

    public override void FixedUpdateNetwork() // FixedUpdate ��Ʈ��ũ �󿡼� �������� �� ����ٰ� ó���� ���ٰſ���.
    {
        // base.FixedUpdateNetwork();

        if (HasStateAuthority == false) return; // �� �÷��̾ ���� �츮�� ������ ������Ʈ�� ���� ������ ������ �־��. 
                                                // �ٵ� �ٸ� Ŭ���̾�Ʈ�� ������ �÷��̾�� �츮�� ������ �����. 
                                                // �� ������ �ǰ� �߿��ѵ�, �ϴ� �ؿ� �ڵ带 �ۼ��� ���Կ�.
                                                // �츮�� �Է��� ������, �ٸ� Ŭ���̾�Ʈ�� �÷��̾� �� �����̸� �ȵ��ݾƿ�.
                                                // �׷��� ��Ʈ��ũ ������Ʈ���� �� ������ �־��. 

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * playerSpeed;

        _cc.Move(move);

        if(move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
}
