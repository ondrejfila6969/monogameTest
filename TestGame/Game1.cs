using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D playerTexture;
    private Vector2 playerPosition;
    private Vector2 playerVelocity;  // Rychlost hráče (počátečně 0)
    private const float gravity = 0.5f;  // Gravitace (akcelerace)
    private bool isOnGround = false;
    private List<Vector2> platformPositions;
    private Rectangle platformSourceRect = new Rectangle(0, 0, 100, 20); // výřez z tilesetu
    
    
    // Rozměry okna
    private int windowWidth;
    private int windowHeight;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Načteme rozměry okna
        windowWidth = _graphics.PreferredBackBufferWidth + 120;
        windowHeight = _graphics.PreferredBackBufferHeight + 120;
        Console.WriteLine(windowHeight);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        platformPositions = new List<Vector2>
        {
            new Vector2(100, 500),
            new Vector2(250, 450),
            new Vector2(400, 400),
            new Vector2(600, 500),
            new Vector2(100, windowHeight - 20) // základní podlaha
        };
        
        // Načtení textury hráče
        playerTexture = Content.Load<Texture2D>("spritesheet");
        playerPosition = new Vector2(200, 200);  // Počáteční pozice
        playerVelocity = new Vector2(0, 0);  // Počáteční rychlost (0)

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyboardState keyboardState = Keyboard.GetState();

        // --- Vodorovný pohyb (A, D)
        float moveSpeed = 3f;
        if (keyboardState.IsKeyDown(Keys.A))
            playerPosition.X -= moveSpeed;
        if (keyboardState.IsKeyDown(Keys.D))
            playerPosition.X += moveSpeed;

        // --- Skákání (W, SPACE)
        float jumpStrength = -10f;
        if (isOnGround && (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space)))
        {
            playerVelocity.Y = jumpStrength;
            isOnGround = false;
        }

        // --- Gravitace
        playerVelocity.Y += gravity;

        // --- Aktualizace pozice hráče
        playerPosition += new Vector2(0, playerVelocity.Y);

        // --- Kontrola spodního okraje okna (dopad)
        if (playerPosition.Y + playerTexture.Height >= windowHeight)
        {
            playerPosition.Y = windowHeight - playerTexture.Height;
            playerVelocity.Y = 0;
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }

        // --- Kontrola horního okraje okna
        if (playerPosition.Y < 0)
        {
            playerPosition.Y = 0;
            playerVelocity.Y = 0;
        }

        // --- Omezení pohybu vlevo a vpravo
        if (playerPosition.X < 0)
            playerPosition.X = 0;
        if (playerPosition.X + playerTexture.Width > windowWidth)
            playerPosition.X = windowWidth - playerTexture.Width;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // --- Vykresli hráče
        Rectangle playerRect = new Rectangle(0, 0, 8, 8);
        _spriteBatch.Draw(playerTexture, playerPosition, playerRect, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
