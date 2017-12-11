namespace Mogo.AI.BT
{
	public sealed class BT101 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT101 _instance = null;
		public static BT101 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT101();

				return _instance;
			}
		}

		private BT101()
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
							node11.AddChild(new Mogo.AI.InSkillCoolDown(4));
							{
								Mogo.AI.SequenceNode node13 = new Mogo.AI.SequenceNode();
								node11.AddChild(node13);
                                node13.AddChild(new Mogo.AI.CastSpell(4, 0));
								node13.AddChild(new Mogo.AI.EnterCD(60000));
							}
						}
						{
							Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
							node10.AddChild(node16);
							node16.AddChild(new Mogo.AI.InSkillCoolDown(3));
							{
								Mogo.AI.SequenceNode node18 = new Mogo.AI.SequenceNode();
								node16.AddChild(node18);
                                node18.AddChild(new Mogo.AI.CastSpell(3, 0));
								node18.AddChild(new Mogo.AI.EnterCD(60000));
							}
						}
						{
							Mogo.AI.SequenceNode node21 = new Mogo.AI.SequenceNode();
							node10.AddChild(node21);
							node21.AddChild(new Mogo.AI.InSkillCoolDown(2));
							{
								Mogo.AI.SequenceNode node23 = new Mogo.AI.SequenceNode();
								node21.AddChild(node23);
                                node23.AddChild(new Mogo.AI.CastSpell(2, 0));
								node23.AddChild(new Mogo.AI.EnterCD(60000));
							}
						}
						{
							Mogo.AI.SequenceNode node26 = new Mogo.AI.SequenceNode();
							node10.AddChild(node26);
							node26.AddChild(new Mogo.AI.InSkillCoolDown(1));
							{
								Mogo.AI.SequenceNode node28 = new Mogo.AI.SequenceNode();
								node26.AddChild(node28);
                                node28.AddChild(new Mogo.AI.CastSpell(1, 0));
								node28.AddChild(new Mogo.AI.EnterCD(60000));
							}
						}
						node10.AddChild(new Mogo.AI.EnterRest(0));
					}
				}
			}
		}
	}
}
