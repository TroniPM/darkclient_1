namespace Mogo.AI.BT
{
	public sealed class BT1006 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT1006 _instance = null;
		public static BT1006 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT1006();

				return _instance;
			}
		}

		private BT1006()
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
					node3.AddChild(new Mogo.AI.IsTargetCanBeAttack());
					{
						Mogo.AI.SelectorNode node8 = new Mogo.AI.SelectorNode();
						node3.AddChild(node8);
						{
							Mogo.AI.SequenceNode node9 = new Mogo.AI.SequenceNode();
							node8.AddChild(node9);
							node9.AddChild(new Mogo.AI.InSkillCoolDown(1));
							{
								Mogo.AI.SelectorNode node11 = new Mogo.AI.SelectorNode();
								node9.AddChild(node11);
								{
									Mogo.AI.SequenceNode node12 = new Mogo.AI.SequenceNode();
									node11.AddChild(node12);
									node12.AddChild(new Mogo.AI.InSkillRange(1));
                                    node12.AddChild(new Mogo.AI.CastSpell(1, 0));
									node12.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
									node11.AddChild(node16);
									node16.AddChild(new Mogo.AI.ChooseCastPoint(1));
									node16.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
                        node8.AddChild(new Mogo.AI.LookOn(500, 200, 1000, 50, 1000, 50, 1000, 50, 1000, 50, 1000, 0, 50, 1));
					}
				}
			}
		}
	}
}
