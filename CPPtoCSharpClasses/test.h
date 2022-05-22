class Derived : public Base
{
	private:
		int int_field;
		AnotherClass class_field;
	public:
		Derived();
		int get_int_field();
		AnotherClass get_class_field();
		void set_class_field(AnotherClass value);
		bool do_work(std::string param1, double param2, 
			YetAnotherClass param3);
		bool operator==(const Derived& other);
		~Derived();
};