namespace Mogo.AI.BT
{
	public sealed class BT1001 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT1001 _instance = null;
		public static BT1001 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT1001();

				return _instance;
			}
		}

		private BT1001()
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
					{
						Mogo.AI.SelectorNode node10 = new Mogo.AI.SelectorNode();
						node3.AddChild(node10);
						{
							Mogo.AI.SequenceNode node11 = new Mogo.AI.SequenceNode();
							node10.AddChild(node11);
							node11.AddChild(new Mogo.AI.InSkillRange(2));
							{
								Mogo.AI.SelectorNode node13 = new Mogo.AI.SelectorNode();
								node11.AddChild(node13);
								{
									Mogo.AI.SequenceNode node14 = new Mogo.AI.SequenceNode();
									node13.AddChild(node14);
									node14.AddChild(new Mogo.AI.InSkillCoolDown(2));
									node14.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,40));
									node14.AddChild(new Mogo.AI.CastSpell(2,0));
									node14.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SequenceNode node19 = new Mogo.AI.SequenceNode();
									node13.AddChild(node19);
									node19.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
									node19.AddChild(new Mogo.AI.Escape(1000));
								}
							}
						}
						{
							Mogo.AI.SequenceNode node22 = new Mogo.AI.SequenceNode();
							node10.AddChild(node22);
							{
								Mogo.AI.Not node23 = new Mogo.AI.Not();
								node22.AddChild(node23);
								node23.Proxy(new Mogo.AI.InSkillRange(2));
							}
							node22.AddChild(new Mogo.AI.InSkillRange(1));
							node22.AddChild(new Mogo.AI.InSkillCoolDown(1));
							node22.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,40));
							node22.AddChild(new Mogo.AI.CastSpell(1,0));
							node22.AddChild(new Mogo.AI.EnterCD(0));
						}
						node10.AddChild(new Mogo.AI.LookOn(600,400,500,10,1000,20,1000,5,1000,5,1000,60,0,1));
					}
				}
			}
		}
	}
}
