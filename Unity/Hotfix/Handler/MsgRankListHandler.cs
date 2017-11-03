using UnityEngine;
using Model;

namespace Hotfix
{
	[MessageHandler((int)Opcode.Response_RankList)]
	public class Response_RankListHandler : AMHandler<Response_RankList>
	{
		protected override void Run(Response_RankList message)
		{
            UIBattleMainComponent.Instance.ProcessRankList(message);
		}
	}
}
