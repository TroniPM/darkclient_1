namespace Mogo.AI.BT
{
	public sealed class BT222 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT222 _instance = null;
		public static BT222 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT222();

				return _instance;
			}
		}

		private BT222()
		{
			{
				Mogo.AI.SelectorNode node1 = new Mogo.AI.SelectorNode();
				this.AddChild(node1);
				node1.AddChild(new Mogo.AI.CmpEnemyNum(Mogo.AI.CmpType.eq,0));
				{
					Mogo.AI.SequenceNode node3 = new Mogo.AI.SequenceNode();
					node1.AddChild(node3);
					{
						Mogo.AI.SelectorNode node4 = new Mogo.AI.SelectorNode();
						node3.AddChild(node4);
						node4.AddChild(new Mogo.AI.HasFightTarget());
						node4.AddChild(new Mogo.AI.AOI(0));
					}
					{
						Mogo.AI.Not node7 = new Mogo.AI.Not();
						node3.AddChild(node7);
						node7.Proxy(new Mogo.AI.ISCD());
					}
					node3.AddChild(new Mogo.AI.IsTargetCanBeAttack());
					node3.AddChild(new Mogo.AI.InSkillRange(1));
					node3.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,100));
					node3.AddChild(new Mogo.AI.Escape(1000));
				}
				node1.AddChild(new Mogo.AI.LookOn(1000,100,0,0,500,2,200,2,200,2,200,94,0,1));
			}
		}
	}
}
