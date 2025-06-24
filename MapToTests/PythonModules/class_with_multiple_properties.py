class TestClassWithMultipleProperties:
    testStr: str
    testInt: int
    testNone: None
    def __init__(self, testStr: str, testInt: int):
        self.testStr = testStr
        self.testInt = testInt
        self.testNone = None

def get_test_class_with_multiple_properties(testStr: str, testInt: int) -> TestClassWithMultipleProperties:
    return TestClassWithMultipleProperties(testStr, testInt)
        