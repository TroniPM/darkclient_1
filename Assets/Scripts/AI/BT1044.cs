namespace Mogo.AI.BT
{
	public sealed class BT1044 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT1044 _instance = null;
		public static BT1044 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT1044();

				return _instance;
			}
		}

		private BT1044()
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
							Mogo.AI.SelectorNode node11 = new Mogo.AI.SelectorNode();
							node10.AddChild(node11);
							{
								Mogo.AI.SequenceNode node12 = new Mogo.AI.SequenceNode();
								node11.AddChild(node12);
								node12.AddChild(new Mogo.AI.InSkillRange(2));
                                node12.AddChild(new Mogo.AI.CastSpell(2, 0));
								node12.AddChild(new Mogo.AI.EnterCD(0));
							}
							{
								Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
								node11.AddChild(node16);
								node16.AddChild(new Mogo.AI.ChooseCastPoint(2));
								node16.AddChild(new Mogo.AI.MoveTo());
							}
						}
						{
							Mogo.AI.SelectorNode node19 = new Mogo.AI.SelectorNode();
							node10.AddChild(node19);
							{
								Mogo.AI.SequenceNode node20 = new Mogo.AI.SequenceNode();
								node19.AddChild(node20);
								node20.AddChild(new Mogo.AI.InSkillRange(1));
                                node20.AddChild(new Mogo.AI.CastSpell(1, 0));
								node20.AddChild(new Mogo.AI.EnterCD(0));
							}
							{
								Mogo.AI.SequenceNode node24 = new Mogo.AI.SequenceNode();
								node19.AddChild(node24);
								node24.AddChild(new Mogo.AI.ChooseCastPoint(1));
								node24.AddChild(new Mogo.AI.MoveTo());
							}
						}
						node10.AddChild(new Mogo.AI.EnterRest(400));
					}
				}
			}
		}
	}
}
