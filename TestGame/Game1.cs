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

    // --- Předpovězená pozice hráče po aplikaci gravitace
    Vector2 predictedPosition = playerPosition + new Vector2(0, playerVelocity.Y);

    // --- Kontrola kolize s platformami
    bool collisionWithPlatform = false;
    foreach (var platformPos in platformPositions)
    {
        Rectangle platformRect = new Rectangle((int)platformPos.X, (int)platformPos.Y, 30, 30); // Platforma 30x30

        // Vytvoření hranic hráče pro detekci všech stran
        Rectangle futurePlayerBounds = new Rectangle((int)playerPosition.X, (int)predictedPosition.Y, 8, 8);

        // --- Kolize s horní stranou platformy
        if (futurePlayerBounds.Bottom > platformRect.Top && futurePlayerBounds.Top < platformRect.Top)
        {
            // Hráč narazil na horní stranu platformy (spadne dolů)
            if (futurePlayerBounds.Right > platformRect.Left && futurePlayerBounds.Left < platformRect.Right)
            {
                playerPosition.Y = platformRect.Top - 8; // Umístíme hráče těsně nad platformu
                playerVelocity.Y = 0;
                isOnGround = true;
                collisionWithPlatform = true;
                break;
            }
        }

        // --- Kolize s dolní stranou platformy
        if (futurePlayerBounds.Top < platformRect.Bottom && futurePlayerBounds.Bottom > platformRect.Bottom)
        {
            // Hráč narazil na spodní stranu platformy (pokud se pohybuje vzhůru)
            if (futurePlayerBounds.Right > platformRect.Left && futurePlayerBounds.Left < platformRect.Right)
            {
                playerPosition.Y = platformRect.Bottom; // Hráč je nad platformou
                playerVelocity.Y = 0;
            }
        }

        // --- Kolize s levým okrajem platformy
        if (futurePlayerBounds.Right > platformRect.Left && futurePlayerBounds.Left < platformRect.Left)
        {
            // Hráč narazil na levý okraj platformy
            if (futurePlayerBounds.Bottom > platformRect.Top && futurePlayerBounds.Top < platformRect.Bottom)
            {
                playerPosition.X = platformRect.Left - 8; // Umístíme hráče vlevo od platformy
                playerVelocity.X = 0; // Zastavíme horizontální pohyb
            }
        }

        // --- Kolize s pravým okrajem platformy
        if (futurePlayerBounds.Left < platformRect.Right && futurePlayerBounds.Right > platformRect.Right)
        {
            // Hráč narazil na pravý okraj platformy
            if (futurePlayerBounds.Bottom > platformRect.Top && futurePlayerBounds.Top < platformRect.Bottom)
            {
                playerPosition.X = platformRect.Right; // Umístíme hráče vpravo od platformy
                playerVelocity.X = 0; // Zastavíme horizontální pohyb
            }
        }
    }

    // --- Pokud hráč není na platformě, aktualizuj jeho pozici (padá)
    if (!collisionWithPlatform)
    {
        playerPosition += new Vector2(0, playerVelocity.Y);
        isOnGround = false;
    }

    // --- Kontrola dopadu na spodní okraj okna
    if (playerPosition.Y + playerTexture.Height >= windowHeight)
    {
        playerPosition.Y = windowHeight - playerTexture.Height;
        playerVelocity.Y = 0;
        isOnGround = true;
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

        // --- Vykresli platformy
        Rectangle platformSource = new Rectangle(0, 8, 30, 30); // výřez dlaždice pod hráčem
        foreach (var platformPos in platformPositions)
        {
            _spriteBatch.Draw(playerTexture, platformPos, platformSource, Color.White);
        }

        // --- Vykresli hráče
        Rectangle playerRect = new Rectangle(0, 0, 8, 8); // výřez hráče
        _spriteBatch.Draw(playerTexture, playerPosition, playerRect, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
