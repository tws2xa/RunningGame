using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems
{
    /*
     * A Template for the system class
     */
    [Serializable()]
    public class SignSystem : GameSystem //Always extend GameSystem. Always have public. Always have [Serializable()].
    {
        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();

        //All systems MUST have a variable holding the level they're contained in
        Level level;

        public System.Drawing.Color signCol = System.Drawing.Color.WhiteSmoke;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SignSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.SIGN_COMPONENT_NAME);


            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents()
        {
            return requiredComponents;
        }

        //Must have this. Same for all Systems.
        public override Level GetActiveLevel()
        {
            return level;
        }

        //You must have an Update.
        //Always read in deltaTime, and only deltaTime (it's the time that's passed since the last frame)
        //Use deltaTime for things like changing velocity or changing position from velocity
        //This is where you do anything that you want to happen every frame.
        //There is a chance that your system won't need to do anything in update. Still have it.
        public override void Update(float deltaTime)
        {
            foreach (Entity e in getApplicableEntities())
            {
                SignComponent signComp = (SignComponent)e.getComponent(GlobalVars.SIGN_COMPONENT_NAME);

                if (!signComp.isActive)
                {
                    continue;
                }
                else //checks to see if player is still colliding with the sign
                {
                    PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );

                    System.Drawing.PointF position = posComp.getLocAsPoint();
                    float xPos = position.X;
                    float yPos = position.Y;
                    System.Drawing.PointF startPoint = new System.Drawing.PointF( xPos - posComp.width / 2, yPos );
                    System.Drawing.PointF endPoint = new System.Drawing.PointF( xPos + posComp.width / 2, yPos );

                    List<Entity> list = level.getCollisionSystem().findObjectsBetweenPoints(startPoint, endPoint); //find all the objects between (hopefully) the ends of the sign

                    bool playerExists = false;
                    foreach ( Entity ent in list ) {
                        if ( ent is Entities.Player )
                        {
                            playerExists = true;
                            break;
                        }
                    }
                    if ( playerExists ) {
                        signComp.isActive = true;
                        level.sysManager.drawSystem.beginConstText( signComp.text, level.sysManager.signSystem.signCol );
                    } else {
                        signComp.isActive = false;
                        level.sysManager.drawSystem.endConstText( signComp.text, signCol, 0.5f );
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------

    }
}
