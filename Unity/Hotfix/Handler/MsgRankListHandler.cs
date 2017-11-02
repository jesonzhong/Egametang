using UnityEngine;
using Model;

namespace Hotfix
{
	[MessageHandler(Opcode.Response_RankList)]
	public class Response_RankListHandler : AMHandler<Response_RankList>
	{
		protected override void Run(Response_RankList message)
		{
            Debug.Log("wahahahhahaha");
            //UIBattleMainComponent.Instance.ProcessRankList(message);
		}
	}
}
