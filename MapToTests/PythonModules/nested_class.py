
class NestedObj:
    number: int
    word: str
    def __init__(self):
        self.number = 123        
        self.word = "Hello" 

class MainObj:
    id: int
    name: str
    nested_obj: NestedObj
    def __init__(self):
        self.id = 1               
        self.name = "Bob"         
        self.nested_obj = NestedObj()  
        
def get_main_obj() -> MainObj:
    return MainObj()