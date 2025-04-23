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
    private Vector2 playerVelocity; 
    private const float gravity = 0.5f;  
    private const float groundLevel = 400f; 
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
    }

    protected override void LoadContent()
    {
        playerTexture = Content.Load<Texture2D>("spritesheet");
        playerPosition = new Vector2(200, 200);  
        playerVelocity = new Vector2(0, 0);

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        playerVelocity.Y += gravity; 
        
        playerPosition += playerVelocity;
        
        if (playerPosition.Y > groundLevel)
        {
            playerPosition.Y = groundLevel;  
            playerVelocity.Y = 0;  
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        
        Rectangle rectangle = new Rectangle(0, 0, 8, 8);
        
        _spriteBatch.Draw(playerTexture, playerPosition, rectangle, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
