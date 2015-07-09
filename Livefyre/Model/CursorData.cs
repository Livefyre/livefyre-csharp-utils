using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Model
{

public class CursorData {
    // may need to be special type
    protected static readonly string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";
    //DateTime.Now.ToString("yyyyMMddHHmmss");

    private string resource;
    private string cursorTime;
    private bool next = false;
    private bool previous = false;
    private int limit;
    
    public CursorData(string resource, int limit, Date startTime) {
        // figure out this-ness 
       // DATE_FORMAT.SetTimeZone(TimeZone.GetTimeZone("UTC"));
        this.resource = resource;
        this.limit = limit;
       // this.cursorTime = startTime == null ? null: DATE_FORMAT.format(startTime);
    }

    public string GetResource() {
        return resource;
    }

    public CursorData SetResource(string resource) {
        this.resource = resource;
        return this;
    }

    public string GetCursorTime() {
        return cursorTime;
    }

    public CursorData SetCursorTime(string newTime) {
        this.cursorTime = newTime;
        return this;
    }

    public CursorData SetCursorTime(DateTime newTime) {
       // this.cursorTime = DATE_FORMAT.format(newTime);
        return this;
    }

    public bool isPrevious() {
        return previous;
    }

    public CursorData SetPrevious(bool previous) {
        this.previous = previous;
        return this;
    }

    public bool isNext() {
        return next;
    }

    public CursorData SetNext(bool next) {
        this.next = next;
        return this;
    }

    public int GetLimit() {
        return limit;
    }

    public CursorData SetLimit(int limit) {
        this.limit = limit;
        return this;
    }
}

}
