


class DifferentCollections:
    numbers: list[int]
    def __init__(self, numbers: list[int]):
        self.numbers = numbers
        
def create_collection(numbers: list[int]) -> DifferentCollections:
    return DifferentCollections(numbers)