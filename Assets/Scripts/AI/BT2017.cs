namespace Mogo.AI.BT
{
	public sealed class BT2017 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT2017 _instance = null;
		public static BT2017 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT2017();

				return _instance;
			}
		}

		private BT2017()
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
							node11.AddChild(new Mogo.AI.InSkillCoolDown(5));
							node11.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
							node11.AddChild(new Mogo.AI.CastSpell(5,0));
							node11.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
							node10.AddChild(node16);
							node16.AddChild(new Mogo.AI.InSkillCoolDown(4));
							node16.AddChild(new Mogo.AI.InSkillRange(4));
							node16.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
							node16.AddChild(new Mogo.AI.CastSpell(4,0));
							node16.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node22 = new Mogo.AI.SequenceNode();
							node10.AddChild(node22);
							node22.AddChild(new Mogo.AI.InSkillCoolDown(3));
							node22.AddChild(new Mogo.AI.InSkillRange(3));
							node22.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
							node22.AddChild(new Mogo.AI.CastSpell(3,0));
							node22.AddChild(new Mogo.AI.EnterCD(200));
						}
						{
							Mogo.AI.SequenceNode node28 = new Mogo.AI.SequenceNode();
							node10.AddChild(node28);
							node28.AddChild(new Mogo.AI.InSkillCoolDown(2));
							node28.AddChild(new Mogo.AI.InSkillRange(2));
							node28.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,40));
							node28.AddChild(new Mogo.AI.CastSpell(2,0));
							node28.AddChild(new Mogo.AI.EnterCD(-4000));
						}
						{
							Mogo.AI.SequenceNode node34 = new Mogo.AI.SequenceNode();
							node10.AddChild(node34);
							node34.AddChild(new Mogo.AI.InSkillCoolDown(1));
							node34.AddChild(new Mogo.AI.InSkillRange(1));
							node34.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,70));
							node34.AddChild(new Mogo.AI.CastSpell(1,0));
							node34.AddChild(new Mogo.AI.EnterCD(200));
						}
						{
							Mogo.AI.SequenceNode node40 = new Mogo.AI.SequenceNode();
							node10.AddChild(node40);
							node40.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,80));
							node40.AddChild(new Mogo.AI.ChooseCastPoint(1));
							node40.AddChild(new Mogo.AI.MoveTo());
						}
						node10.AddChild(new Mogo.AI.EnterRest(200));
					}
				}
			}
		}
	}
}
