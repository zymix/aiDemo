using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class CellSpacePartition<T> where T:BaseEntity{
    class Cell<U> where U:BaseEntity{
        public LinkedList<U> members;
        public Bounds bbox;
        public Cell(in Vector3 center, in Vector3 size){
            bbox = new Bounds(center, size);
            members = new LinkedList<U>();
        }
        public bool IsEmpty(){
            return members.Count == 0;
        }
    }
    List<Cell<T>> _cells;
    List<T> _neighbors; //保存上一次与cells求交后得到的entities结果集，注意容器会复用
    public List<T> getLastNeighbors(){
        return _neighbors;
    }
    int _curNeighborIdx;//当前遍历_neighbors的下标
    int _endNeighborIdx;//_neighbors的结束下标
    Vector3 _spaceSize;
    //Vector3 _spaceCenter;
    Vector3 _leftBottomPos;
    Vector3 _rigtTopPos;
    int _cellNumX;
    int _cellNumY;
    int _cellNumZ;

    float _cellSizeX;
    float _cellSizeY;
    float _cellSizeZ;
    public CellSpacePartition(Vector3 center, Vector3 size, Vector3 cellNums, int maxEntitys){
        _cells = new List<Cell<T>>();
        _cellNumX = (int)cellNums.x;
        _cellNumY = (int)cellNums.y;
        _cellNumZ = (int)cellNums.z;
        _spaceSize = size;
        _leftBottomPos = new Vector3(center.x-size.x*0.5f,center.y-size.y*0.5f,center.z-size.z*0.5f);
        _rigtTopPos = new Vector3(center.x+size.x*0.5f,center.y+size.y*0.5f,center.z+size.z*0.5f);
        _cellSizeX = _spaceSize.x / _cellNumX;
        _cellSizeY = _spaceSize.y / _cellNumY;
        _cellSizeZ = _spaceSize.z / _cellNumZ;
        //从左下角向右上角的方向建立cell,向右向前向上的顺序
        
        float sy = _cellSizeY + _leftBottomPos.y;
        for(int y = 0; y < _cellNumY; ++y){
            float sz = _cellSizeZ * 0.5f + _leftBottomPos.z;

            for(int z = 0;z<_cellNumZ; ++z){
                
                float sx = _cellSizeX * 0.5f + _leftBottomPos.x;
                for(int x = 0;x<_cellNumX; ++x){
                    _cells.Add(new Cell<T>(new Vector3(sx, sy, sz), new Vector3(_cellSizeX, _cellSizeY, _cellSizeZ)));
                    sx += _cellSizeX;
                }
                sz += _cellSizeZ;
            }
            sy += _cellSizeY;
        }

        _neighbors = new List<T>();
        _neighbors.Capacity = _cellNumX * _cellNumY * _cellNumZ;
        _curNeighborIdx = 0;
    }

    // public T Begin(){
    //     _curNeighborIdx = 0;
    //     return _neighbors[0];
    // }

    // public T Next(){
    //     return _neighbors[++_curNeighborIdx];
    // }

    // public void Set(T neighbor){
    //     _neighbors[_curNeighborIdx] = neighbor;
    // }

    // public bool IsEnd(){
    //     return _curNeighborIdx>=_neighbors.Count ||_neighbors[_curNeighborIdx] == null;
    // }

    public void CalculateNeighbors(Vector3 center, float queryRadius){
        Bounds bounds = new Bounds(center, new Vector3(queryRadius,queryRadius,queryRadius));
        float sqrRadius = queryRadius * queryRadius;
        _curNeighborIdx = 0;
        foreach(var cell in _cells){
            if(cell.IsEmpty() || !cell.bbox.Intersects(bounds)){
                continue;
            }
            foreach(var member in cell.members){
                float x = member.pos.x - center.x;
                float y = member.pos.y - center.y;
                float z = member.pos.z - center.z;
                if(x*x+y*y+z*z > sqrRadius){
                    if(_neighbors.Count>=_curNeighborIdx){
                        _neighbors.Add(member);
                    }else{
                        _neighbors[_curNeighborIdx] = member;
                    }
                    ++_curNeighborIdx;
                }
            }
        }
        if(_neighbors.Count <_curNeighborIdx){
            _neighbors[_curNeighborIdx] = null;
        }
    }

    public void ClearCells(){
        foreach(var cell in _cells){
            cell.members.Clear();
        }
    }

    public int PositionToIndex(in Vector3 pos){
        int ix = (int)(_cellNumX * (pos.x - _leftBottomPos.x) / _spaceSize.x);
        int iy = (int)(_cellNumY * (pos.y - _leftBottomPos.y) / _spaceSize.y);
        int iz = (int)(_cellNumZ * (pos.z - _leftBottomPos.z) / _spaceSize.z);
        int idx = ix + iy * _cellNumX + iz*(_cellNumX * _cellNumY);
        if(idx >= _cells.Count|| idx<0){
            return _cells.Count-1;
        }
        return idx;
    }

    public void AddEntity(T entity){
        if(null == entity){
            return;
        }
        int idx = PositionToIndex(entity.pos);
        _cells[idx].members.AddLast(entity);
        _DebugObjectMaterial(entity, idx);
    }

    public void UpdateEntity(T entity, Vector3 lastPos){
        if(null == entity){
            return;
        }
        int oldIdx = PositionToIndex(lastPos);
        int curIdx = PositionToIndex(entity.pos);
        if(oldIdx == curIdx){
            return;
        }
        _cells[oldIdx].members.Remove(entity);
        _cells[curIdx].members.AddLast(entity);
        _DebugObjectMaterial(entity, curIdx);
    }

    public void DebugDrawOn(){
        foreach(var cell in _cells){
            Vector3 lb = cell.bbox.min;
            Vector3 rt = cell.bbox.max;
            var gameObject = new GameObject();
            gameObject.transform.position.Set(0,3,0);
            var debugLine = gameObject.AddComponent<LineRenderer>();
            debugLine.material = new Material(Shader.Find("Unlit/Color"));
            debugLine.material.color = Color.green;
            debugLine.startWidth = 0.05f;
            debugLine.endWidth =  0.05f;
            debugLine.loop = true;
            debugLine.positionCount = 4;
            debugLine.SetPositions(new Vector3[]{
                new Vector3(lb.x, 3, lb.z), new Vector3(rt.x, 3, lb.z),
                new Vector3(rt.x, 3, rt.z), new Vector3(lb.x, 3, rt.z)
            });

            // Debug.DrawLine(new Vector3(lb.x, lb.y, lb.z), new Vector3(rt.x, lb.y, lb.z), Color.green, 2, false);
            // Debug.DrawLine(new Vector3(lb.x, lb.y, lb.z), new Vector3(lb.x, lb.y, rt.z), Color.green, 2, false);
            // Debug.DrawLine(new Vector3(lb.x, lb.y, rt.z), new Vector3(rt.x, lb.y, rt.z), Color.green, 2, false);
            // Debug.DrawLine(new Vector3(rt.x, lb.y, lb.z), new Vector3(rt.x, lb.y, rt.z), Color.green, 2, false);
        }
    }

    private void _DebugObjectMaterial(T entity, int idx) {
        int iy = idx / (_cellNumX * _cellNumZ);
        int iz = (idx % (_cellNumX * _cellNumZ)) / _cellNumX;
        int ix = (idx % (_cellNumX * _cellNumZ)) % _cellNumX;
        Color  clr;
        if(iz > ix){
            clr = new Color(0.0f, 0.0f, idx*1.0f / _cells.Count);
        }else{
            clr = new Color(idx*1.0f / _cells.Count, 0.0f, 0.0f);
        }
        // clr = new Color((float)idx / _cellNumX, (float)iy / _cellNumY, (float)iz / _cellNumZ);
        // Debug.LogFormat("{0}:{1},{2},{3}, color:{4}", idx, ix,iy,iz, clr);
        entity.view.GetComponent<Renderer>().material.color = clr;
    }
}