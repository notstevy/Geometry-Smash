using System;
using System.Data;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using EntitySystem;
using Geometry_Smash;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Component 
{
    public Entity Parent;

    public bool NeedsUpdate;
    public Action UpdateFunction;

    public Component(Entity parent, bool needsUpdate = false, Action updateFunction = null)
    {
        Parent = parent;

        NeedsUpdate = needsUpdate;
        UpdateFunction = updateFunction;
    }
}

public class GravityComponent : Component 
{
    public float Acceleration;
    public float YVel;

    public GravityComponent(Entity parent, float acceleration) : base(parent, true, null)
    {
        Acceleration = acceleration;
        UpdateFunction = Update;
    }
    
    public void Update() 
    {
        YVel += Acceleration;
        Parent.Velocity.Y += YVel;
    }
}

public class ColliderComponent : Component 
{
    public bool isStatic;
    public bool OnGround;

    public Action ResetFunction;

    public ColliderComponent(Entity parent, Action resetFunction, bool isstatic = true) : base(parent, !isstatic, null)
    {
        UpdateFunction = Update;
        isStatic = isstatic;
        ResetFunction = resetFunction;
    }
    
    public void Update() 
    {
        OnGround = false;

        Vector2 thisPosition = Parent.Position + Parent.Velocity;
        float thisBottom = thisPosition.Y + (Parent.Texture.Height * Parent.Scale) + Parent.Velocity.Y;
        float thisRight = thisPosition.X + (Parent.Texture.Width * Parent.Scale) + Parent.Velocity.X;

        float previousBottom = thisPosition.Y + (Parent.Texture.Height * Parent.Scale);
        float previousRight = thisPosition.X + (Parent.Texture.Width * Parent.Scale);

        GravityComponent GravityComponente = Parent.GetComponent<GravityComponent>(); 

        for (int i = 0; i < Game1.CurrLevel.Collider.Count; i++) 
        {
            ColliderComponent other = Game1.CurrLevel.Collider[i];
            if (other == this) { continue; }
            
            Vector2 otherPosition = other.Parent.Position;
            float otherBottom = otherPosition.Y + other.Parent.Texture.Height * other.Parent.Scale;
            float otherRight = otherPosition.X + other.Parent.Texture.Width * other.Parent.Scale;
            
            if (thisBottom > otherPosition.Y && thisRight > otherPosition.X && thisPosition.X < otherRight && thisPosition.Y < otherBottom) 
            {
                float UpdatedRight = thisRight;
                float UpdatedBottom = thisBottom;
            
                if (previousBottom - 1 <= otherPosition.Y) 
                {
                    if (GravityComponente != null) 
                    {   
                        GravityComponente.YVel = 0f;
                    } 

                    Parent.Position.Y = otherPosition.Y - Parent.Texture.Height * Parent.Scale; 
                    Parent.Velocity.Y = 0f;

                    UpdatedRight = otherPosition.Y - Parent.Texture.Height * Parent.Scale + (Parent.Texture.Width * Parent.Scale);
                    
                    OnGround = true;
                }

                if (UpdatedRight > otherPosition.Y && otherPosition.Y < UpdatedRight !& thisBottom - otherPosition.Y > 10f) 
                {
                    ResetFunction.Invoke();
                }
            }
        }
    }
}

public class CharacterControllerComponent : Component 
{
    public float JumpStrength;

    public CharacterControllerComponent(Entity parent, float jumpStrength) : base(parent, true, null) 
    {
        UpdateFunction = Update;
        JumpStrength = jumpStrength;
    }
    
    public void Update() 
    {
        ColliderComponent ColliderComponent = Parent.GetComponent<ColliderComponent>();
        
        if (ColliderComponent != null) 
        {
            KeyboardState KeyboardState = Keyboard.GetState();
        
            if (ColliderComponent.OnGround == true) 
            {
                Console.WriteLine("CanJump");
                if (KeyboardState.IsKeyDown(Keys.Space)) 
                {
                    Parent.Velocity.Y -= JumpStrength;
                }
            }
        }
    }
}