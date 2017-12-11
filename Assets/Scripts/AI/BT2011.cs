namespace Mogo.AI.BT
{
	public sealed class BT2011 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT2011 _instance = null;
		public static BT2011 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT2011();

				return _instance;
			}
		}

		private BT2011()
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
							node11.AddChild(new Mogo.AI.InSkillCoolDown(6));
							node11.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,100));
							node11.AddChild(new Mogo.AI.CastSpell(6,0));
							node11.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
							node10.AddChild(node16);
							node16.AddChild(new Mogo.AI.InSkillCoolDown(5));
							{
								Mogo.AI.Not node18 = new Mogo.AI.Not();
								node16.AddChild(node18);
								node18.Proxy(new Mogo.AI.InSkillRange(2));
							}
							{
								Mogo.AI.SelectorNode node20 = new Mogo.AI.SelectorNode();
								node16.AddChild(node20);
								{
									Mogo.AI.SequenceNode node21 = new Mogo.AI.SequenceNode();
									node20.AddChild(node21);
									node21.AddChild(new Mogo.AI.InSkillRange(5));
									node21.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
									node21.AddChild(new Mogo.AI.CastSpell(5,0));
									node21.AddChild(new Mogo.AI.EnterCD(-1500));
								}
								{
									Mogo.AI.SequenceNode node26 = new Mogo.AI.SequenceNode();
									node20.AddChild(node26);
									node26.AddChild(new Mogo.AI.ChooseCastPoint(5));
									node26.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SequenceNode node29 = new Mogo.AI.SequenceNode();
							node10.AddChild(node29);
							node29.AddChild(new Mogo.AI.InSkillCoolDown(4));
							node29.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
							node29.AddChild(new Mogo.AI.CastSpell(4,0));
							node29.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node34 = new Mogo.AI.SequenceNode();
							node10.AddChild(node34);
							node34.AddChild(new Mogo.AI.InSkillCoolDown(3));
							node34.AddChild(new Mogo.AI.InSkillRange(3));
							node34.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
							node34.AddChild(new Mogo.AI.CastSpell(3,0));
							node34.AddChild(new Mogo.AI.EnterCD(500));
						}
						{
							Mogo.AI.SequenceNode node40 = new Mogo.AI.SequenceNode();
							node10.AddChild(node40);
							node40.AddChild(new Mogo.AI.InSkillCoolDown(2));
							node40.AddChild(new Mogo.AI.InSkillRange(2));
							node40.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
							node40.AddChild(new Mogo.AI.CastSpell(2,0));
							node40.AddChild(new Mogo.AI.EnterCD(500));
						}
						{
							Mogo.AI.SequenceNode node46 = new Mogo.AI.SequenceNode();
							node10.AddChild(node46);
							node46.AddChild(new Mogo.AI.InSkillCoolDown(1));
							node46.AddChild(new Mogo.AI.InSkillRange(1));
							node46.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
							node46.AddChild(new Mogo.AI.CastSpell(1,0));
							node46.AddChild(new Mogo.AI.EnterCD(500));
						}
						{
							Mogo.AI.SequenceNode node52 = new Mogo.AI.SequenceNode();
							node10.AddChild(node52);
							node52.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,80));
							node52.AddChild(new Mogo.AI.ChooseCastPoint(1));
							node52.AddChild(new Mogo.AI.MoveTo());
						}
						node10.AddChild(new Mogo.AI.EnterRest(500));
					}
				}
			}
		}
	}
}
