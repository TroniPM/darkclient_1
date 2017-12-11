namespace Mogo.AI.BT
{
	public sealed class BT10104 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT10104 _instance = null;
		public static BT10104 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT10104();

				return _instance;
			}
		}

		private BT10104()
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
							node10.AddChild(new Mogo.AI.InSkillRange(5));
							node10.AddChild(new Mogo.AI.InSkillCoolDown(5));
							node10.AddChild(new Mogo.AI.CastSpell(5,0));
							node10.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node15 = new Mogo.AI.SequenceNode();
							node9.AddChild(node15);
							node15.AddChild(new Mogo.AI.InSkillCoolDown(6));
							node15.AddChild(new Mogo.AI.InSkillRange(6));
							node15.AddChild(new Mogo.AI.CastSpell(6,0));
							node15.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node20 = new Mogo.AI.SequenceNode();
							node9.AddChild(node20);
							node20.AddChild(new Mogo.AI.InSkillCoolDown(4));
							node20.AddChild(new Mogo.AI.InSkillRange(4));
							node20.AddChild(new Mogo.AI.CastSpell(4,0));
							node20.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node25 = new Mogo.AI.SequenceNode();
							node9.AddChild(node25);
							node25.AddChild(new Mogo.AI.InSkillRange(1));
							node25.AddChild(new Mogo.AI.CastSpell(1,0));
							node25.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node29 = new Mogo.AI.SequenceNode();
							node9.AddChild(node29);
							node29.AddChild(new Mogo.AI.ChooseCastPoint(1));
							node29.AddChild(new Mogo.AI.MoveTo());
						}
						//node9.AddChild(new Mogo.AI.EnterRest(1000));
					}
				}
			}
		}
	}
}
