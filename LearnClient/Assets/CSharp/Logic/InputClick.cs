using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Entitas;

public class InputClick : MonoBehaviour
{
    public static Vector3 CurClickPos;

    public Camera mainCamera;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                Vector3 pos = Input.mousePosition;
                Ray ray = mainCamera.ScreenPointToRay(pos);
                RaycastHit hint = new RaycastHit();
                if (Physics.Raycast(ray, out hint, 1000) == true)
                {
                    //GameEntity gameEntity = EntityMgr.Instance.GetGameEntity(1);
                    //MoveComp moveComp = gameEntity.GetComponent(GameComponentsLookup.MoveComp) as MoveComp;
                    //moveComp.DestPos = hint.point;
                    BattleCommand command = new BattleCommand();
                    command.EntityId = 1;
                    command.CommandType = BattleCommandType.Move;
                    command.MoveInfo = new BattleCommand.CommandMoveInfo();
                    command.MoveInfo.DestPos = new Vector3(hint.point.x, 0, hint.point.z);

                    BattleLoop.Instance.AddCommand(command);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W) == true)
        {
            BattleCommand command = new BattleCommand();
            command.EntityId = 1;
            command.CommandType = BattleCommandType.PutSkill;
            command.PutSkillInfo = new BattleCommand.CommandPutSkillInfo();
            command.PutSkillInfo.SkillId = 101;
            BattleLoop.Instance.AddCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.E) == true)
        {
            BattleCommand command = new BattleCommand();
            command.EntityId = 1;
            command.CommandType = BattleCommandType.PutSkill;
            command.PutSkillInfo = new BattleCommand.CommandPutSkillInfo();
            command.PutSkillInfo.SkillId = 102;
            BattleLoop.Instance.AddCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.R) == true)
        {
            BattleCommand command = new BattleCommand();
            command.EntityId = 1;
            command.CommandType = BattleCommandType.PutSkill;
            command.PutSkillInfo = new BattleCommand.CommandPutSkillInfo();
            command.PutSkillInfo.SkillId = 103;
            BattleLoop.Instance.AddCommand(command);
        }
    }
}
