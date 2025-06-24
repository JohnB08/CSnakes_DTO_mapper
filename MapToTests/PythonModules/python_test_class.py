class Greeter:
    name: str
    def __init__(self, name: str):
        self.name = name
        

def create_greeter(name: str) -> Greeter:
    return Greeter(name)