namespace Mogo.AI.BT
{
	public sealed class BT10000 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT10000 _instance = null;
		public static BT10000 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT10000();

				return _instance;
			}
		}

		private BT10000()
		{
			{
				Mogo.AI.SelectorNode node1 = new Mogo.AI.SelectorNode();
				this.AddChild(node1);
				{
					Mogo.AI.SequenceNode node2 = new Mogo.AI.SequenceNode();
					node1.AddChild(node2);
					{
						Mogo.AI.SelectorNode node3 = new Mogo.AI.SelectorNode();
						node2.AddChild(node3);
						node3.AddChild(new Mogo.AI.MercenaryAOI());
						node3.AddChild(new Mogo.AI.FollowOwner());
					}
					{
						Mogo.AI.Not node6 = new Mogo.AI.Not();
						node2.AddChild(node6);
						node6.Proxy(new Mogo.AI.ISCD());
					}
					node2.AddChild(new Mogo.AI.IsTargetCanBeAttack());
					{
						Mogo.AI.SelectorNode node9 = new Mogo.AI.SelectorNode();
						node2.AddChild(node9);
						{
							Mogo.AI.SequenceNode node10 = new Mogo.AI.SequenceNode();
							node9.AddChild(node10);
							node10.AddChild(new Mogo.AI.InSkillRange(1));
							node10.AddChild(new Mogo.AI.CastSpell(1,0));
							node10.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node14 = new Mogo.AI.SequenceNode();
							node9.AddChild(node14);
							node14.AddChild(new Mogo.AI.CastSpell(5,0));
							node14.AddChild(new Mogo.AI.EnterCD(0));
						}
						node9.AddChild(new Mogo.AI.LookOn(500,100,1000,33,2000,0,1500,35,1500,36,1000,0,38,1));
					}
				}
			}
		}
	}
}
