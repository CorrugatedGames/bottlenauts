public partial class Referee : Node
{
  #region Signals
  [Signal]
  public delegate void CharacterInstancedEventHandler (int playerNumber, Vector3 position);

  [Signal]
  public delegate void CharacterBombPlacedEventHandler (int playerNumber, Vector3 bombPosition);

  [Signal]
  public delegate void CharacterDiedEventHandler (int playerNumber);
  #endregion

  public void ConnectCharacterSignals (Character character)
  {
    int playerNumber = character.PlayerNumber;

    // add more signals here - character.CoinCollected, character.PowerupCollected, etc
    character.Instanced += (position) => EmitSignal(SignalName.CharacterInstanced, playerNumber, position);
    character.BombPlaced += (bombPosition) => EmitSignal(SignalName.CharacterBombPlaced, playerNumber, bombPosition);
    character.Died += () => EmitSignal(SignalName.CharacterDied, playerNumber);
  }
}