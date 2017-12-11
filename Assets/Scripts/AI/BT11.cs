namespace Mogo.AI.BT
{
	public sealed class BT11 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT11 _instance = null;
		public static BT11 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT11();

				return _instance;
			}
		}

		private BT11()
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
						node4.AddChild(new Mogo.AI.AOI(30));
					}
					{
						Mogo.AI.Not node7 = new Mogo.AI.Not();
						node3.AddChild(node7);
						node7.Proxy(new Mogo.AI.ISCD());
					}
					node3.AddChild(new Mogo.AI.IsTargetCanBeAttack());
					{
						Mogo.AI.SelectorNode node10 = new Mogo.AI.SelectorNode();
						node3.AddChild(node10);
						{
							Mogo.AI.SequenceNode node11 = new Mogo.AI.SequenceNode();
							node10.AddChild(node11);
							node11.AddChild(new Mogo.AI.InSkillCoolDown(2));
							node11.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,80));
                            node11.AddChild(new Mogo.AI.CastSpell(2, 0));
							node11.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
							node10.AddChild(node16);
							node16.AddChild(new Mogo.AI.InSkillCoolDown(1));
							node16.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,60));
                            node16.AddChild(new Mogo.AI.CastSpell(1, 0));
							node16.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node21 = new Mogo.AI.SequenceNode();
							node10.AddChild(node21);
							node21.AddChild(new Mogo.AI.EnterRest(300));
						}
					}
				}
			}
		}
	}
}
