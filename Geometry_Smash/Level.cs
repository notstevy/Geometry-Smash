using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

public class Level 
{
    public Vector2 StartPos;

    public Dictionary<Microsoft.Xna.Framework.Vector2, Entity> BlockMap = new Dictionary<Microsoft.Xna.Framework.Vector2, Entity>();

    public List<Entity> Entities = new List<Entity>();
    public List<ColliderComponent> Collider = new List<ColliderComponent>();
    
    public Level(Vector2 startPos, Dictionary<Microsoft.Xna.Framework.Vector2, Entity> blockMap, List<Entity> entities, List<ColliderComponent> collider)
    {
        StartPos = startPos;
        BlockMap = blockMap;
        Entities = entities;
        Collider = collider;
    }
}