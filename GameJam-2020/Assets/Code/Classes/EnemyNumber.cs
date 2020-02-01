using Sirenix.OdinInspector;

[System.Serializable]
public class EnemyNumber {
    public Enemy enemyType;
    [HorizontalGroup("Group1")]
    public int minAmount;
    [HorizontalGroup("Group1")]
    public int maxAmount;
}