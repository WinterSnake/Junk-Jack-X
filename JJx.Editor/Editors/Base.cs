/*
	Junk Jack X Tools: Editor
	- Editor

	Written By: Ryan Smith
*/

public abstract class EditorBase
{
	/* Constructor */
	public EditorBase(string filePath) { this._FilePath = filePath; }
	/* Instance Methods */
	public abstract void Draw();
	public abstract void Update(float deltaTime);
	/* Properties */
	protected readonly string _FilePath;
}
