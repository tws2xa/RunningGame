using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RunningGame.Entities;
using RunningGame.Level_Editor;
using System.Reflection;
using System.Collections;
using RunningGame.Components;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace RunningGame {
    public partial class FormEditor : Form {

        CreationGame creationGame;
        delegate void refreshPropertiesListDelegate();

        public List<string> blockedComponentVarNames = new List<string>()
        {
            "componentName",
            "myEntity",
            "colSys",
            "startingX",
            "startingY",
            "startingWidth",
            "startingHeight"
        };

        public bool allowKBackingFields = false;

        public FormEditor() {
            InitializeComponent();
        }

        private void FormEditor_Load( object sender, EventArgs e ) {

            Type type = typeof( Entity );
            foreach ( Type t in this.GetType().Assembly.GetTypes() ) {
                if ( type.IsAssignableFrom( t ) && type != t && t != typeof( EntityTemplate ) && t != typeof( ProtoEntity ) ) {
                    EntityListItem i = new EntityListItem( t );
                    lstEntities.Items.Add( i );
                }
            }

            //Add key handlers to the main panel
            ( pnlMain as Control ).KeyPress += new KeyPressEventHandler( panelKeyPressEventHandler );
            ( pnlMain as Control ).KeyDown += new KeyEventHandler( panelKeyDownEventHandler );
            ( pnlMain as Control ).KeyUp += new KeyEventHandler( panelKeyUpEventHandler );
            ( pnlMain as Control ).MouseMove += new MouseEventHandler( MouseMoveHandler );
            ( pnlMain as Control ).MouseLeave += new EventHandler( MouseLeaveHandler );

            pnlMainContainer.AutoScroll = true;

            changeMainPanelSize( pnlMainContainer.Width, pnlMainContainer.Height );
            creationGame = new CreationGame( pnlMain.CreateGraphics(), pnlMain.Width, pnlMain.Height );
            creationGame.getCurrentLevel().vars.editForm = this;

        }

        private void listBox1_SelectedIndexChanged( object sender, EventArgs e ) {

            //Deselect any selected entities
            txtVar.Text = "";
            if ( creationGame.getCurrentLevel().vars.selectedEntity != null ) {
                creationGame.getCurrentLevel().sysManager.inputManSystem.deselectEntity();
            }

            if ( creationGame.getCurrentLevel().vars.protoEntity != null ) {
                creationGame.getCurrentLevel().removeEntity( creationGame.getCurrentLevel().vars.protoEntity );
                creationGame.getCurrentLevel().vars.protoEntity = null;
            }

            if ( lstEntities.SelectedIndex != -1 ) {
                EntityListItem item = ( EntityListItem )lstEntities.Items[lstEntities.SelectedIndex];
                ProtoEntity p = new ProtoEntity( creationGame.getCurrentLevel(), item.myType );
                creationGame.getCurrentLevel().addEntity( p.randId, p );
                creationGame.getCurrentLevel().vars.protoEntity = p;
            }

            pnlMain.Focus();

        }

        public void refreshEntityPropertiesList() {
            //Get on the proper thread
            if ( InvokeRequired ) {
                Invoke( new refreshPropertiesListDelegate( refreshEntityPropertiesList ) );
            } else {
                if ( creationGame.getCurrentLevel().vars.selectedEntity != null ) {
                    Entity e = creationGame.getCurrentLevel().vars.selectedEntity;

                    txtId.Text = e.randId.ToString();

                    lstSelectedEntProperties.Items.Clear();

                    foreach ( FieldInfo f in e.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) ) {
                        lstSelectedEntProperties.Items.Add( new FieldInfoListItem( f, e ) );
                    }

                    foreach ( Component c in new List<Component>() /*e.getComponents()*/) {
                        foreach ( FieldInfo f in c.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) ) {
                            if ( !blockedComponentVarNames.Contains( f.Name ) )
                                if ( !f.Name.Contains( "k__BackingField" ) || allowKBackingFields )
                                    lstSelectedEntProperties.Items.Add( new FieldInfoListItem( f, c ) );
                        }
                    }
                } else {
                    lstSelectedEntProperties.Items.Clear();
                }
            }
        }

        private void lstSelectedEntProperties_SelectedIndexChanged( object sender, EventArgs e ) {
            FieldInfoListItem item = ( FieldInfoListItem )lstSelectedEntProperties.Items[lstSelectedEntProperties.SelectedIndex];
            lblVar.Text = item.fieldInfo.Name + ":";
            if ( item.obj != null )
                txtVar.Text = item.fieldInfo.GetValue( item.obj ).ToString();
        }

        public void updateFieldInfoFromText() {
            //If no entity or property is selected, return
            if ( lstSelectedEntProperties.SelectedIndex == -1 )
                return;
            if ( creationGame.getCurrentLevel().vars.selectedEntity == null )
                return;

            FieldInfoListItem item = ( FieldInfoListItem )lstSelectedEntProperties.Items[lstSelectedEntProperties.SelectedIndex];
            //Have to use movement system if position component
            if ( item.obj.GetType() == typeof( PositionComponent ) ) {
                PositionComponent posComp = ( PositionComponent )item.obj;
                if ( item.fieldInfo.Name == "x" ) {
                    creationGame.getCurrentLevel().getMovementSystem().changePosition( posComp, float.Parse( txtVar.Text ), posComp.y, false );
                    posComp.startingX = float.Parse( txtVar.Text );
                } else if ( item.fieldInfo.Name == "y" ) {
                    creationGame.getCurrentLevel().getMovementSystem().changePosition( posComp, posComp.x, float.Parse( txtVar.Text ), false );
                    posComp.startingY = float.Parse( txtVar.Text );
                } else if ( item.fieldInfo.Name == "width" ) {
                    creationGame.getCurrentLevel().getMovementSystem().changeWidth( posComp, float.Parse( txtVar.Text ) );
                    posComp.startingWidth = float.Parse( txtVar.Text );

                    Entity e = posComp.myEntity;
                    if ( e.hasComponent( GlobalVars.DRAW_COMPONENT_NAME ) ) {
                        DrawComponent drawComp = ( DrawComponent )e.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                        if ( drawComp.sizeLocked ) {
                            drawComp.resizeImages( int.Parse( txtVar.Text ), ( int )drawComp.height );
                        }
                    }
                } else if ( item.fieldInfo.Name == "height" ) {
                    creationGame.getCurrentLevel().getMovementSystem().changeHeight( posComp, float.Parse( txtVar.Text ) );
                    posComp.startingHeight = float.Parse( txtVar.Text );

                    Entity e = posComp.myEntity;
                    if ( e.hasComponent( GlobalVars.DRAW_COMPONENT_NAME ) ) {
                        DrawComponent drawComp = ( DrawComponent )e.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                        if ( drawComp.sizeLocked ) {
                            drawComp.resizeImages( ( int )drawComp.width, int.Parse( txtVar.Text ) );
                        }
                    }
                }
            } else if ( item.fieldInfo.FieldType == typeof( float ) ) {
                item.fieldInfo.SetValue( item.obj, float.Parse( txtVar.Text ) );
            } else if ( item.fieldInfo.FieldType == typeof( int ) ) {
                item.fieldInfo.SetValue( item.obj, int.Parse( txtVar.Text ) );
            } else if ( item.fieldInfo.FieldType == typeof( bool ) ) {
                if ( txtVar.Text.ToLower() == "false" || txtVar.Text == "0" ) {
                    item.fieldInfo.SetValue( item.obj, false );
                } else {
                    item.fieldInfo.SetValue( item.obj, true );
                }
            } else if ( item.fieldInfo.FieldType != typeof( string ) ) {
                item.fieldInfo.SetValue( item.obj, new StringConverter().ConvertTo( txtVar.Text, item.fieldInfo.FieldType ) );
            } else {
                item.fieldInfo.SetValue( item.obj, txtVar.Text );
            }
            txtVar.SelectionStart = 0;
            txtVar.SelectionLength = 0;
            pnlMain.Focus();
        }

        public void txtChangeAccept( object sender, KeyEventArgs e ) {
            if ( e.KeyData == Keys.Enter ) {
                updateFieldInfoFromText();
                txtVar.BackColor = Color.White;
            } else {
                txtVar.BackColor = Color.PaleVioletRed;
            }
        }

        public void changeMainPanelSize( int width, int height ) {
            pnlMain.Width = width;
            pnlMain.Height = height;
        }

        private void btnLoadFromPaint_Click( object sender, EventArgs e ) {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk( object sender, CancelEventArgs e ) {
            creationGame.close();
            Size s = Bitmap.FromFile( openFileDialog1.FileName ).Size;
            changeMainPanelSize( ( int )( s.Width * GlobalVars.LEVEL_READER_TILE_WIDTH ), ( int )( s.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT ) );
            creationGame = new CreationGame( pnlMain.CreateGraphics(), ( int )( s.Width * GlobalVars.LEVEL_READER_TILE_WIDTH ), ( int )( s.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT ), openFileDialog1.FileName );
            creationGame.getCurrentLevel().vars.editForm = this;
            btnLoadFromPaint.Enabled = false;
        }


        private void openFileDialog2_FileOk( object sender, CancelEventArgs e ) {
            creationGame.close();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = new FileStream( openFileDialog2.FileName, FileMode.Open );

            List<Object> ents = ( List<Object> )bf.Deserialize( f );

            float levelWidth = ( float )ents[0];
            float levelHeight = ( float )ents[1];

            changeMainPanelSize( ( int )levelWidth, ( int )levelHeight );

            creationGame = new CreationGame( pnlMain.CreateGraphics(), ( int )levelWidth, ( int )levelHeight );
            creationGame.getCurrentLevel().vars.editForm = this;

            //if (!sysManagerInit) sysManager = new SystemManager(this);
            //sysManagerInit = true;

            for ( int i = 2; i < ents.Count; i++ ) {
                Entity ent = ( Entity )ents[i];
                ent.level = creationGame.getCurrentLevel();
                creationGame.getCurrentLevel().addEntity( ent.randId, ent );
            }
        }

        private void FormEditor_FormClosing( object sender, FormClosingEventArgs e ) {
            creationGame.close();
        }
        private void chkLockToGrid_CheckedChanged( object sender, EventArgs e ) {
            creationGame.getCurrentLevel().vars.gridLock = chkLockToGrid.Checked;
        }

        private void btnCreate_Click( object sender, EventArgs e ) {
            if ( txtFileName.Text == "" ) {
                txtFileName.BackColor = Color.Red;
                return;
            }

            creationGame.getCurrentLevel().paused = true; //STAHP THE GAEM!

            string fileName = txtFileName.Text + ".bin";

            //XmlSerializer serializer = new XmlSerializer(typeof(List<Entity>));
            //StreamWriter stream = new StreamWriter(fileName);

            BinaryFormatter serializer = new BinaryFormatter();
            FileStream stream = new FileStream( fileName, FileMode.Create );

            List<Object> ents = new List<Object>();

            ents.Add( creationGame.getCurrentLevel().levelWidth );
            ents.Add( creationGame.getCurrentLevel().levelHeight );

            foreach ( Entity ent in GlobalVars.nonGroundEntities.Values ) {
                ents.Add( ent );
            }
            foreach ( Entity ent in GlobalVars.groundEntities.Values ) {
                ents.Add( ent );
            }

            serializer.Serialize( stream, ents );

            Console.WriteLine( "Level File Location: " + stream.Name );

            stream.Close();

            txtFileName.BackColor = Color.Green;

        }


        private void txtFileName_TextChanged( object sender, EventArgs e ) {
            txtFileName.BackColor = Color.White;
        }

        //Input
        private void MouseMoveHandler( object sender, MouseEventArgs e ) {
            creationGame.MouseMoved( e );
        }
        private void MouseLeaveHandler( object sender, EventArgs e ) {
            creationGame.MouseLeave( e );
        }
        private void pnlMain_MouseClick( object sender, MouseEventArgs e ) {
            pnlMain.Focus();
            creationGame.getCurrentLevel().sysManager.MouseClick( e );
        }

        public void panelKeyPressEventHandler( Object sender, KeyPressEventArgs e ) {
            creationGame.KeyPressed( e );
        }
        public void panelKeyUpEventHandler( Object sender, KeyEventArgs e ) {
            creationGame.KeyUp( e );
        }
        public void panelKeyDownEventHandler( Object sender, KeyEventArgs e ) {
            //Console.WriteLine(e.KeyData + " is down");
            creationGame.KeyDown( e );
        }

        private void btnLoadFromBinary_Click( object sender, EventArgs e ) {
            openFileDialog2.ShowDialog();
        }

        private void txtVar_TextChanged( object sender, EventArgs e ) {

        }

    }
}
