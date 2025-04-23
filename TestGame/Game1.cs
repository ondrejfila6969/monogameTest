using System;
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
        windowWidth = _graphics.PreferredBackBufferWidth;
        windowHeight = _graphics.PreferredBackBufferHeight + 120;
        Console.WriteLine(windowHeight);
        base.Initialize();
    }

    protected override void LoadContent()
    {
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

        // Aplikování gravitace na rychlost
        playerVelocity.Y += gravity;  // Zvyšování rychlosti směrem dolů

        // Aktualizace pozice hráče (pozice = pozice + rychlost)
        playerPosition += playerVelocity;

        // Zastavení hráče, pokud dosáhne spodního okraje okna
        if (playerPosition.Y + playerTexture.Height > windowHeight)
        {
            playerPosition.Y = windowHeight - playerTexture.Height;  // Nastaví pozici tak, že spodní okraj je na okraji obrazovky
            playerVelocity.Y = 0;  // Zastaví pohyb směrem dolů
        }

        // Zastavení hráče, pokud dosáhne horního okraje okna
        if (playerPosition.Y < 0)
        {
            playerPosition.Y = 0;  // Nastavení hráče na horní okraj
            playerVelocity.Y = 0;  // Zastavení pohybu směrem nahoru
        }

        // Omezení pohybu na ose X (pokud hráč vyjde z okna)
        if (playerPosition.X < 0)
        {
            playerPosition.X = 0;  // Levý okraj okna
        }
        if (playerPosition.X + playerTexture.Width > windowWidth)
        {
            playerPosition.X = windowWidth - playerTexture.Width;  // Pravý okraj okna
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Nastavíme velikost spritu na 8x8 pixelů
        Rectangle rectangle = new Rectangle(0, 0, 8, 8);
        
        // Vykreslíme hráče na obrazovku
        _spriteBatch.Draw(playerTexture, playerPosition, rectangle, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
