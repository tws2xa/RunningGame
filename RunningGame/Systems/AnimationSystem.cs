using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems
{
    class AnimationSystem:GameSystem
    {

        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public AnimationSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.DRAW_COMPONENT_NAME); //Draw
            requiredComponents.Add(GlobalVars.ANIMATION_COMPONENT_NAME); //Animated


            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override ArrayList getRequiredComponents()
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
                AnimationComponent animComp = (AnimationComponent)e.getComponent(GlobalVars.ANIMATION_COMPONENT_NAME);
                DrawComponent drawComp = (DrawComponent)e.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

                if (animComp.animationOn)
                {

                    if (animComp.timeUntilNextFrame <= 0)
                    {
                        drawComp.getSprite().currentImageIndex++;
                        if (drawComp.getSprite().currentImageIndex >= drawComp.getSprite().getNumImages())
                        {
                            drawComp.getSprite().currentImageIndex = 0;
                            if (animComp.pauseIndefinitelyAfterCycle)
                            {
                                animComp.animationOn = false;
                                drawComp.getSprite().currentImageIndex = 0;
                            }
                            animComp.timeUntilNextFrame = animComp.animationFrameTime + animComp.pauseTimeAfterCycle;
                            drawComp.getSprite().currentImageIndex = 0;
                        }
                        else
                        {
                            animComp.timeUntilNextFrame = animComp.animationFrameTime;
                        }
                    }
                    else
                    {
                        animComp.timeUntilNextFrame -= deltaTime;
                    }

                }

            }

        }

        //----------------------------------------------------------------------------------------
        public void setFrame(Entity e, int frameNum)
        {
            DrawComponent drawComp = (DrawComponent)e.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            if (frameNum < drawComp.getSprite().getNumImages())
            {
                drawComp.getSprite().currentImageIndex = frameNum;
            }
            else
            {
                Console.WriteLine("Trying to change " + e + " sprite to nonexistant frame: " + frameNum);
            }
        }

        public void pauseAnimationForTime(Entity e, float time)
        {
            AnimationComponent animComp = (AnimationComponent)e.getComponent(GlobalVars.ANIMATION_COMPONENT_NAME);
            pauseAnimationForTime(animComp, time);
        }
        public void pauseAnimationForTime(AnimationComponent animComp, float time)
        {
            animComp.timeUntilNextFrame += time;
        }


        public void pauseAnimationIndefinitely(Entity e)
        {
            AnimationComponent animComp = (AnimationComponent)e.getComponent(GlobalVars.ANIMATION_COMPONENT_NAME);
            pauseAnimationIndefinitely(animComp);
        }
        public void pauseAnimationIndefinitely(AnimationComponent animComp)
        {
            animComp.animationOn = false;
        }

        public void restartAnimation(Entity e)
        {
            AnimationComponent animComp = (AnimationComponent)e.getComponent(GlobalVars.ANIMATION_COMPONENT_NAME);
            restartAnimation(animComp);
        }
        public void restartAnimation(AnimationComponent animComp)
        {
            animComp.animationOn = true;
            animComp.timeUntilNextFrame = animComp.animationFrameTime;
        }
    }
}
