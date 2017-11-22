using UnityEngine;
using Model;

namespace Hotfix
{
	[MessageHandler((int)Opcode.Actor_RankList)]
	public class Response_RankListHandler : AMHandler<Actor_RankList>
	{
		protected override void Run(Actor_RankList message)
		{
            UIBattleMainComponent.Instance.ProcessRankList(message);
		}
	}
}
