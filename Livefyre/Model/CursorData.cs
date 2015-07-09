using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Model
{

public class CursorData {
    protected static readonly string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";

    private string resource;
    private string cursorTime;
    private bool next = false;
    private bool previous = false;
    private int limit;
    
    public CursorData(string resource, int limit, DateTime startTime) {
        this.resource = resource;
        this.limit = limit;

        // try catch meh!?
        string start = startTime.ToUniversalTime().ToString(DATE_FORMAT);
        this.cursorTime = start == null ? null : start;
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
        this.cursorTime = newTime.ToUniversalTime().ToString(DATE_FORMAT);
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
