# Object Pooling

## Basic usage
Create a game object pool
```c#
_missilePool = GameObjectPool.CreatePool(_missilePrefab.gameObject);
```
Allocate initial instance amount
```c#
_missilePool.Allocate(5);
```
Spawn an object
```c#
var obj = _missilePool.Pop(position, rotation, parent)
```
Despawn the object
```c#
_missilePool.Push(obj)
```

------------------------------------

## GameObjectPool

GameObjectPools are stored in `DontDestroyOnLoad` scene. 

### GameObjectPool.CreatePool

It is used for creating GameObject pools by giving a prefab gameObject. 
Returned value is a `GameObjectPool` with useful helper methods.

```c#
GameObjectPool _missilePool = GameObjectPool.CreatePool(_missilePrefab.gameObject);
```

### GameObjectPool.Allocate

When you create a `GameObjectPool`, it has no instance in it yet.
You can optionally allocate new objects in it before using the pool.
If you don't allocate and the pool is empty, new instance of the object is created when you try to `Pop` an instance.

```c#
_missilePool.Allocate(5);
```

### GameObjectPool.Pop

To get an instance from the pool, use `Pop` method (instead of using `Object.Instantiate`).
If there is no instance in the pool, it will create a new instance of the object (expanding the pool size)

```c#
var obj = _missilePool.Pop(position, rotation, parent)
```

### GameObjectPool.Push

To move the instance to the pool, use `Push` method (instead of using `Object.Destroy`).
This will not destroy the object, it will move it to the Pool container which is in the `DontDestroyOnLoad` scene.

```c#
_missilePool.Push(obj)
```

### GameObjectPool.DisposePool

Use this method if you want to remove all the instances in the pool, excluding popped objects.
You can use the same pool after disposing.

```c#
_missilePool.DisposePool()
```

### GameObjectPool.GetPrefab

It returns the original prefab reference

```c#
var prefab = _missilePool.GetPrefab()
```

### GameObjectPool.GetPoolOfPrefab

Returns the pool of a prefab. If it is not found, it returns null.

```c#
var pool = GameObjectPool.GetPoolOfPrefab(_missilePrefab)
```

### GameObjectPool.GetPoolOfInstance

Returns the pool of an instance. If it is not found, it returns null.

```c#
var pool = GameObjectPool.GetPoolOfInstance(missile)
```

------------------------------------

## ObjectPool

It is similar to `GameObjectPool` but for regular objects.

### ObjectPool.CreatePool

It is used for creating object pools by giving a generic type.
Returned value is a `ObjectPool` with useful helper methods.

```c#
var _missileModelPool = ObjectPool<MissileModel>.CreatePool();
```

### ObjectPool.Allocate

When you create a `ObjectPool`, it has no instance in it yet.
You can optionally allocate new objects in it before using the pool.
If you don't allocate and the pool is empty, new instance of the object is created when you try to `Pop` an instance.

```c#
_missileModelPool.Allocate(5);
```

Optionally, you can pass constructor arguments for new instances

```c#
var model = _missileModelPool.Allocate(5, _missileManager, _audioManager);
```

### ObjectPool.Pop

To get an instance from the pool, use `Pop` method (instead of using `new` operator).
If there is no instance in the pool, it will create a new instance of the object (expanding the pool size)

```c#
var model = _missileModelPool.Pop();
```

Optionally, you can pass constructor arguments for new instances

```c#
var model = _missileModelPool.Pop(_missileManager, _audioManager);
```

### ObjectPool.Push

To move the instance to the pool, use `Push` method. This will prevent garbage collection.

```c#
_missileModelPool.Push(model)
```

### ObjectPool.DisposePool

Use this method if you want to remove all the instances in the pool, excluding popped objects.
You can use the same pool after disposing.

```c#
_missileModelPool.DisposePool()
```

------------------------------------

## Events

You can subscribe to pool events of `ObjectPool` and `GameObjectPool`.

```c#
GameObjectPool missilePool = GameObjectPool.CreatePool(_missilePrefab.gameObject);

missilePool.OnInstanceCreated += (instance) => { ... };
missilePool.OnInstancePopped += (instance) => { ... };
missilePool.OnInstancePushed += (instance) => { ... };
missilePool.OnInstanceDisposed += (instance) => { ... };
missilePool.OnPoolDisposed += () => { ... };

missilePool.Allocate(5);
```

```c#
ObjectPool missileModelPool = ObjectPool.CreatePool<MissileModel>();

missileModelPool.OnInstanceCreated += (instance) => { ... };
missileModelPool.OnInstancePopped += (instance) => { ... };
missileModelPool.OnInstancePushed += (instance) => { ... };
missileModelPool.OnInstanceDisposed += (instance) => { ... };
missileModelPool.OnPoolDisposed += () => { ... };

missileModelPool.Allocate(5);
```

------------------------------------

## PushToPoolAfterDelay component

It is a helper monobehavior component to push an object to its pool after a delay.
You can attach this component on a prefab and set the delay from the inspector. 
It sets a coroutine and waits for given duration to push the object.