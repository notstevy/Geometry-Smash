using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EntitySystem;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using MonoGame.Extended;

namespace Geometry_Smash;

public class Game1 : Game
{
    private List<Texture2D> Blocks = new List<Texture2D>();
    private int CurrBlock = 2;


    private bool Debug = false;


    public Vector2 CamPos;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Entity Cube;
    
    private double _elapsedTime;
    private int _frameCounter;
    private int _fps;

    private SpriteFont font;

    public static Level CurrLevel;

    private bool LevelEditor = true;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        //IsFixedTimeStep = false;
        //_graphics.SynchronizeWithVerticalRetrace = false;
    }

    protected override void Initialize()
    {
        CurrLevel = new Level(new System.Numerics.Vector2(0, 0), new Dictionary<Microsoft.Xna.Framework.Vector2, Entity>(), new List<Entity>(), new List<ColliderComponent>());
        base.Initialize();
    }

    protected override void LoadContent()
    { 
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Cube = EntityUtils.CreateEntity(new Vector2(CurrLevel.StartPos.X, CurrLevel.StartPos.Y), Content.Load<Texture2D>("Gometry"), 3f);

        Cube.AddComponent(new GravityComponent(Cube, 0.2f));
        Cube.AddComponent(new ColliderComponent(Cube, ResetLevel, null, false));
        Cube.AddComponent(new CharacterControllerComponent(Cube, 25));

        Blocks.Add(Content.Load<Texture2D>("DefaultBlock"));
        Blocks.Add(Content.Load<Texture2D>("GradientBlock"));
        Blocks.Add(Content.Load<Texture2D>("Spike"));
        Blocks.Add(Content.Load<Texture2D>("RandomBlock"));
        Blocks.Add(Content.Load<Texture2D>("SmolSpike"));
        
        font = Content.Load<SpriteFont>("font");
    }


    private MouseState PreviousMouseState;
    private KeyboardState PreviousKeyboardState;
    public static int StartDelay;
    
    protected override void Update(GameTime gameTime)
    {
        MouseState MouseState = Mouse.GetState();
        KeyboardState KeyboardState = Keyboard.GetState();
    
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        _frameCounter++;

        if (_elapsedTime >= 1.0)
        {
            _fps = _frameCounter;
            _frameCounter = 0;
            _elapsedTime = 0;

            //Console.WriteLine($"FPS: {_fps}");
        }

        if (LevelEditor) 
        {
            if (KeyboardState.IsKeyDown(Keys.W)) 
            {
                CamPos.Y += 30f;
            }
            if (KeyboardState.IsKeyDown(Keys.S)) 
            {
                CamPos.Y -= 30f;
            }
            if (KeyboardState.IsKeyDown(Keys.A)) 
            {
                CamPos.X += 30f;
            }
            if (KeyboardState.IsKeyDown(Keys.D)) 
            {
                CamPos.X -= 30f;
            }
            
            if (MouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed) 
            {
                PlaceBlock(MouseState.Position.X, MouseState.Position.Y);
            }
            
            if (MouseState.RightButton == ButtonState.Released && PreviousMouseState.RightButton == ButtonState.Pressed) 
            {
                RemoveBlock(MouseState.Position.X, MouseState.Position.Y);
            }
            
            if (MouseState.MiddleButton == ButtonState.Released && PreviousMouseState.MiddleButton == ButtonState.Pressed) 
            {
                Cube.Position.X = MouseState.Position.X - CamPos.X;
                Cube.Position.Y = MouseState.Position.Y - CamPos.Y;
            }
        }
        
        
        if (KeyboardState.IsKeyUp(Keys.F) && PreviousKeyboardState.IsKeyDown(Keys.F)) 
        {
            if (LevelEditor == true) 
            {
                ResetLevel();
            }
            else 
            {
                LevelEditor = true;
                CamPos = CurrLevel.StartPos + new System.Numerics.Vector2((_graphics.PreferredBackBufferWidth / 2) - 200, _graphics.PreferredBackBufferHeight / 2 - 50);
            }
        }
        
        if (KeyboardState.IsKeyUp(Keys.Up) && PreviousKeyboardState.IsKeyDown(Keys.Up)) 
        {
            if (CurrBlock + 1 != Blocks.Count) 
            {
                CurrBlock++;
            }
        }
        if (KeyboardState.IsKeyUp(Keys.Down) && PreviousKeyboardState.IsKeyDown(Keys.Down)) 
        {
            if (CurrBlock - 1 != -1)
            {
                CurrBlock--;
            }
        }

        //Cube.Position.X = Mouse.GetState().Position.X;
        //Cube.Position.Y = Mouse.GetState().Position.Y;

        if (!LevelEditor) 
        {
            if (StartDelay > 0) 
            {
                StartDelay--;
            }
            else 
            {
                EntityUtils.TickEntities();
                Cube.Velocity.X += 1f;
                
                if (CamPos.Y > -Cube.Position.Y + 240) 
                {
                    CamPos.Y -= 1f * MathF.Abs(-Cube.Position.Y + 200 - CamPos.Y) / 10;
                }
                if (CamPos.Y < -Cube.Position.Y + 160) 
                {
                    CamPos.Y += 1f * MathF.Abs(-Cube.Position.Y + 200 - CamPos.Y) / 10;
                }
            }  
            
            CamPos.X = -Cube.Position.X + 100;          
        }
        else 
        {
            Cube.Hidden = true;
        }

        PreviousMouseState = MouseState;
        PreviousKeyboardState = KeyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        EntityUtils.DrawEntities(_spriteBatch, CamPos);
        
        if (LevelEditor) 
        {
            _spriteBatch.Draw(Content.Load<Texture2D>("Gometry"), CurrLevel.StartPos + CamPos, null, Color.White, 0f, new Vector2(), 3f, SpriteEffects.None, 0f);

            _spriteBatch.DrawString(font, "Level Editor", new Vector2(20, 20), Color.White);

            _spriteBatch.Draw(Blocks[CurrBlock], new Vector2(20, 50), null, Color.White, 0f, new Vector2(0, 0), 3f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(font, CurrBlock.ToString(), new Vector2(80, 60), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }


    public void PlaceBlock(float x, float y) 
    {
        float gridSize = 48;
        float adjustedX = (float)Math.Floor((x - CamPos.X) / gridSize) * gridSize + 24;
        float adjustedY = (float)Math.Floor((y - CamPos.Y) / gridSize) * gridSize + 24;

        Vector2 Position = new Vector2(adjustedX, adjustedY);
        
        if (!CurrLevel.BlockMap.ContainsKey(Position)) 
        {
            Entity CreatedEntity = EntityUtils.CreateEntity(new Vector2(adjustedX, adjustedY), Blocks[CurrBlock], 3f);
            
            if (CurrBlock == 2) 
            {
                CreatedEntity.AddComponent(new ColliderComponent(CreatedEntity, ResetLevel, new System.Drawing.RectangleF(50f, 14f, 20, 30)));
            }
            else 
            {
                CreatedEntity.AddComponent(new ColliderComponent(CreatedEntity, ResetLevel));
            }

            CurrLevel.BlockMap[Position] = CreatedEntity;
        }
    }
    
    public void RemoveBlock(float x, float y) 
    {
        float gridSize = 48;
        float adjustedX = (float)Math.Floor((x - CamPos.X) / gridSize) * gridSize + 24;
        float adjustedY = (float)Math.Floor((y - CamPos.Y) / gridSize) * gridSize + 24;

        Vector2 Position = new Vector2(adjustedX, adjustedY);
        
        if (CurrLevel.BlockMap.ContainsKey(Position)) 
        {
            EntityUtils.RemoveEntity(CurrLevel.BlockMap[Position]);
            CurrLevel.BlockMap.Remove(Position);
        }
    }
    
    public void ResetLevel() 
    {
        StartDelay = 100;
        LevelEditor = false;
        Cube.Hidden = false;
        
        Cube.Position = CurrLevel.StartPos + new System.Numerics.Vector2(24, 24);
        GravityComponent g = Cube.GetComponent<GravityComponent>();
        if (g != null) 
        {
            g.YVel = 0f;
        }
        
        Cube.Velocity = Vector2.Zero;
    }
}
