using CrusadeTracker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusadeTracker.Domain.Battles;

public sealed class BattleParticipant
{
    public UserId PlayerId { get; private set; }
    public ForceId ForceId { get; private set; }
    public BattleResult Result { get; private set; }
    public string? ForceNameSnapshot { get; private set; }

    private BattleParticipant() { }

    public BattleParticipant(
        UserId playerId,
        ForceId forceId,
        string? forceNameSnapshot = null)
    {
        PlayerId = playerId;
        ForceId = forceId;
        ForceNameSnapshot = forceNameSnapshot;
    }

    public void SetResult(BattleResult result)
    {
        Result = result;
    }
}
