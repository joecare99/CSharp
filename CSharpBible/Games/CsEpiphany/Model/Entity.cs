using CsEpiphany.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CsEpiphany.Model;

public abstract class Entity : IEntity
{
    // private decl.
    private void m_set_position_x(int x) { }

    private void m_set_position_y(int y) { }

    protected bool m_just_checked;

    protected Entity_Handle m_id;

    protected Entity_Type m_type;

    protected int m_position_x;

    protected int m_position_y;
    protected int m_speed;

    protected Sprite m_sprite;


    //True if an entity exists - not used
    protected bool m_exists;

    protected Level current_level;


    public Entity();

    public Entity_Handle get_id() => m_id;

    public void set_id(Entity_Handle handle) => m_id = handle;

    public int get_position_x() => m_position_x;

    public int get_position_y() => m_position_y;

    public Entity_Type get_type() => m_type;

    public Sprite get_sprite() => m_sprite;

    public void set_speed(int speed) => m_speed = speed;

    public void set_checked(bool check) => m_just_checked = check;

    public void set_type(Entity_Type type) => m_type = type;

    //moving function: calls the correct move_<dir>() function
    public void move(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                move_up();
                break;
            case Direction.DOWN:
                move_down();
                break;
            case Direction.RIGHT:
                move_right();
                break;
            case Direction.LEFT:
                move_left();
                break;
            default:
                break;
        }
    }

    public bool set_position(int x, int y)
    {
        (m_position_x, m_position_y) = (x, y);
        return true;
    }

    public bool set_initial_position(int x, int y) {
        (m_position_x, m_position_y) = (x, y);
        return true;
    }

    // Moving functions - one for every direction
    public void move_up() => m_position_y -= m_speed;

    public void move_down() => m_position_y += m_speed;

    public void move_right() => m_position_x += m_speed;

    public void move_left() => m_position_x -= m_speed;

    public bool exists() { return m_exists; }

    public void kill() { }

    // Virtual functions

    // this function need to be overloaded in the derivative classes.
    //It is called by Game::move_all() for every existing object
    //and generally it contains some checks and calling to moving_functions
    public abstract void check_and_do();
    //	virtual bool pass_on_me(Direction d=STOP)=0;
    public abstract bool player_pressing_up(Entity_Handle down_entity);
    public abstract bool player_pressing_left(Entity_Handle right_entity);
    public abstract bool player_pressing_right(Entity_Handle left_entity);
    public abstract bool player_pressing_down(Entity_Handle up_entity);
    public abstract bool hit_from_up(Entity_Handle ntt);
    public abstract bool explode();
	public abstract bool roll_on_me();
		
	~Entity() { }
}
